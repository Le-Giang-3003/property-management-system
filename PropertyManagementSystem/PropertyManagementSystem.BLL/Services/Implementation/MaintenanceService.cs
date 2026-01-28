using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.BLL.DTOs.Maintenance;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRepository _maintenanceRepo;
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public MaintenanceService(
            IMaintenanceRepository maintenanceRepo,
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _maintenanceRepo = maintenanceRepo;
            _context = context;
            _environment = environment;
        }

        // Tenant Operations
        public async Task<MaintenanceRequestDto> CreateRequestAsync(CreateMaintenanceRequestDto dto, int tenantId)
        {
            // Generate request number
            var latestNumber = await _maintenanceRepo.GetLatestRequestNumberAsync();
            var requestNumber = GenerateRequestNumber(latestNumber);

            var request = new MaintenanceRequest
            {
                PropertyId = dto.PropertyId,
                RequestedBy = tenantId,
                RequestNumber = requestNumber,
                Category = dto.Category,
                Priority = dto.Priority,
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                RepairDate = dto.RepairDate,
                TimeFrom = dto.TimeFrom,
                TimeTo = dto.TimeTo,
                Status = "Pending",
                RequestDate = DateTime.UtcNow,
                ResolutionNotes = "",
                TechnicianStatus = "",
                TenantFeedback = "",
                ReasonRejectLandlord = "",
                ReasonRejectTechnician = "",
                TechnicianNote = ""
            };

            var createdRequest = await _maintenanceRepo.CreateAsync(request);

            // Handle image uploads
            if (dto.Images != null && dto.Images.Any())
            {
                foreach (var image in dto.Images)
                {
                    var imagePath = await SaveImageAsync(image);
                    var maintenanceImage = new MaintenanceImage
                    {
                        RequestId = createdRequest.RequestId,
                        ImageUrl = imagePath,
                        ImagePath = imagePath,
                        Caption = "",
                        ImageType = "Before",
                        UploadedBy = tenantId,
                        UploadedAt = DateTime.UtcNow
                    };
                    await _maintenanceRepo.AddImageAsync(maintenanceImage);
                }
            }

            return await GetRequestByIdAsync(createdRequest.RequestId);
        }

        public async Task<bool> CancelRequestAsync(int requestId, int tenantId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(requestId);
            if (request == null || request.RequestedBy != tenantId)
                return false;

            if (request.Status == "Completed")
                return false;

            return await _maintenanceRepo.CancelRequestAsync(requestId);
        }

        public async Task<bool> RateMaintenanceAsync(RateMaintenanceDto dto, int tenantId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(dto.RequestId);
            if (request == null || request.RequestedBy != tenantId)
                return false;

            if (request.Status != "Completed")
                return false;

            return await _maintenanceRepo.RateMaintenanceAsync(dto.RequestId, dto.Rating, dto.TenantFeedback);
        }

        public async Task<List<MaintenanceRequestDto>> GetTenantRequestsAsync(int tenantId, string status = null)
        {
            var requests = await _maintenanceRepo.GetByTenantIdAsync(tenantId);

            if (!string.IsNullOrEmpty(status))
            {
                requests = requests.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return MapToDto(requests);
        }

        public async Task<MaintenanceStatsDto> GetTenantStatsAsync(int tenantId)
        {
            var stats = await _maintenanceRepo.GetRequestStatsByTenantAsync(tenantId);
            return new MaintenanceStatsDto
            {
                TotalRequests = stats["Total"],
                PendingRequests = stats["Pending"],
                InProgressRequests = stats["InProgress"] + stats["Assigned"],
                CompletedRequests = stats["Completed"],
                CancelledRequests = stats["Cancelled"],
                RejectedRequests = stats["Rejected"]
            };
        }

        // Landlord Operations
        public async Task<List<MaintenanceRequestDto>> GetLandlordRequestsAsync(int landlordId, string status = null)
        {
            var requests = await _maintenanceRepo.GetByLandlordIdAsync(landlordId);

            if (!string.IsNullOrEmpty(status))
            {
                requests = requests.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return MapToDto(requests);
        }

        public async Task<bool> RejectRequestAsync(RejectMaintenanceRequestDto dto, int landlordId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(dto.RequestId);
            if (request == null || request.Property.LandlordId != landlordId)
                return false;

            if (request.Status != "Pending")
                return false;

            request.Status = "Rejected";
            request.ReasonRejectLandlord = dto.Reason;
            //request.ResolutionNotes = dto.Reason;
            request.ClosedBy = landlordId;
            request.ClosedDate = DateTime.UtcNow;

            await _maintenanceRepo.UpdateAsync(request);

            // Add comment with rejection reason
            await AddCommentAsync(dto.RequestId, landlordId, $"Request rejected. Reason: {dto.Reason}");

            return true;
        }

        public async Task<bool> AssignTechnicianAsync(AssignTechnicianDto dto, int landlordId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(dto.RequestId);
            if (request == null || request.Property.LandlordId != landlordId)
                return false;

            if (request.Status != "Pending")
                return false;

            request.AssignedTo = dto.TechnicianId;
            request.AssignedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            request.Status = "InProgress";
            request.TechnicianStatus = "Pending";

            await _maintenanceRepo.UpdateAsync(request);
            return true;
        }

        public async Task<bool> CloseRequestAsync(int requestId, int landlordId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(requestId);
            if (request == null || request.Property.LandlordId != landlordId)
                return false;

            request.Status = "Closed";
            request.ClosedBy = landlordId;
            request.ClosedDate = DateTime.UtcNow;

            await _maintenanceRepo.UpdateAsync(request);
            return true;
        }

        public async Task<bool> ConfirmCompletionAsync(int requestId, int landlordId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(requestId);
            if (request == null || request.Property.LandlordId != landlordId)
                return false;

            if (request.Status != "InProgress")
                return false;

            request.ClosedDate = DateTime.UtcNow;
            request.ClosedBy = landlordId;
            request.Status = "Completed";

            await _maintenanceRepo.UpdateAsync(request);
            return true;
        }

        public async Task<MaintenanceStatsDto> GetLandlordStatsAsync(int landlordId)
        {
            var stats = await _maintenanceRepo.GetRequestStatsByLandlordAsync(landlordId);
            return new MaintenanceStatsDto
            {
                TotalRequests = stats["Total"],
                PendingRequests = stats["Pending"],
                InProgressRequests = stats["InProgress"] + stats["Assigned"],
                CompletedRequests = stats["Completed"],
                CancelledRequests = stats["Cancelled"],
                RejectedRequests = stats["Rejected"]
            };
        }

        public async Task<List<UserDto>> GetAvailableTechniciansAsync()
        {
            var technicians = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.RoleName == "Technician") && u.IsActive)
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    FullName = u.FullName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                })
                .ToListAsync();

            return technicians;
        }

        // Technician Operations
        public async Task<List<MaintenanceRequestDto>> GetTechnicianRequestsAsync(int technicianId, string status = null)
        {
            var requests = await _maintenanceRepo.GetByTechnicianIdAsync(technicianId);

            if (!string.IsNullOrEmpty(status))
            {
                requests = requests.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return MapToDto(requests);
        }

        public async Task<bool> RespondToAssignmentAsync(TechnicianResponseDto dto, int technicianId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(dto.RequestId);
            if (request == null || request.AssignedTo != technicianId)
                return false;

            request.TechnicianStatus = dto.Status;
            request.EstimatedCost = dto.EstimatedCost;

            if (dto.Status == "Accepted")
            {
                request.TechnicianStatus = "Accepted";
                request.TechnicianNote = dto.Notes;
            }
            else if (dto.Status == "Rejected")
            {
                request.TechnicianStatus = "Rejected";
                request.ReasonRejectTechnician = dto.RejectionReason ?? dto.Notes;
                request.Status = "Pending";
                request.AssignedTo = null;
                request.AssignedDate = null;
            }

            // Add comment if notes provided
            if (!string.IsNullOrEmpty(dto.Notes))
            {
                await AddCommentAsync(dto.RequestId, technicianId, dto.Notes);
            }

            if (!string.IsNullOrEmpty(dto.RejectionReason))
            {
                await AddCommentAsync(dto.RequestId, technicianId, $"Assignment rejected. Reason: {dto.RejectionReason}");
            }

            await _maintenanceRepo.UpdateAsync(request);
            return true;
        }

        public async Task<bool> CompleteMaintenanceAsync(CompleteMaintenanceDto dto, int technicianId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(dto.RequestId);
            if (request == null || request.AssignedTo != technicianId)
                return false;

            request.ActualCost = dto.ActualCost;
            request.ResolutionNotes = dto.ResolutionNotes ?? "";
            request.CompletedDate = DateTime.UtcNow;
            request.TechnicianStatus = "Completed";
            request.Status = "Completed";
            request.ClosedBy = technicianId;
            request.ClosedDate = DateTime.UtcNow;

            await _maintenanceRepo.UpdateAsync(request);
            return true;
        }

        public async Task<bool> UpdateRequestStatusAsync(int requestId, string status, int technicianId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(requestId);
            if (request == null || request.AssignedTo != technicianId)
                return false;

            var allowed = new[] { "InProgress", "OnHold" };
            if (string.IsNullOrEmpty(status) || !allowed.Contains(status, StringComparer.OrdinalIgnoreCase))
                return false;

            request.Status = status;
            await _maintenanceRepo.UpdateAsync(request);
            return true;
        }

        public async Task<MaintenanceStatsDto> GetTechnicianStatsAsync(int technicianId)
        {
            var stats = await _maintenanceRepo.GetRequestStatsByTechnicianAsync(technicianId);
            return new MaintenanceStatsDto
            {
                TotalRequests = stats["Total"],
                PendingRequests = stats["Pending"],
                InProgressRequests = stats["InProgress"],
                CompletedRequests = stats["Completed"],
                CancelledRequests = 0
            };
        }

        // Common Operations
        public async Task<MaintenanceRequestDto> GetRequestByIdAsync(int requestId)
        {
            var request = await _maintenanceRepo.GetByIdAsync(requestId);
            if (request == null)
                return null;

            return MapToDto(new List<MaintenanceRequest> { request }).FirstOrDefault();
        }

        public async Task<bool> AddCommentAsync(int requestId, int userId, string comment)
        {
            var maintenanceComment = new MaintenanceComment
            {
                RequestId = requestId,
                UserId = userId,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            await _maintenanceRepo.AddCommentAsync(maintenanceComment);
            return true;
        }

        // Helper Methods
        private string GenerateRequestNumber(string latestNumber)
        {
            if (string.IsNullOrEmpty(latestNumber))
                return "MR-00001";

            var numberPart = latestNumber.Split('-')[1];
            var nextNumber = int.Parse(numberPart) + 1;
            return $"MR-{nextNumber:D5}";
        }

        private async Task<string> SaveImageAsync(Microsoft.AspNetCore.Http.IFormFile image)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "maintenance");
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return $"/uploads/maintenance/{uniqueFileName}";
        }

        private List<MaintenanceRequestDto> MapToDto(List<MaintenanceRequest> requests)
        {
            return requests.Select(r => new MaintenanceRequestDto
            {
                RequestId = r.RequestId,
                PropertyId = r.PropertyId,
                PropertyName = r.Property?.Name,
                PropertyAddress = r.Property?.Address,
                RequestedBy = r.RequestedBy,
                TenantName = r.Tenant?.FullName,
                TenantEmail = r.Tenant?.Email,
                TenantPhone = r.Tenant?.PhoneNumber,
                AssignedTo = r.AssignedTo,
                TechnicianName = r.Technician?.FullName,
                TechnicianEmail = r.Technician?.Email,
                TechnicianPhone = r.Technician?.PhoneNumber,
                RequestNumber = r.RequestNumber,
                Category = r.Category,
                Priority = r.Priority,
                Title = r.Title,
                Description = r.Description,
                Location = r.Location,
                Status = r.Status,
                TechnicianStatus = r.TechnicianStatus,
                TechnicianNote = r.TechnicianNote,
                RequestDate = r.RequestDate,
                AssignedDate = r.AssignedDate,
                RepairDate = r.RepairDate,
                TimeFrom = r.TimeFrom,
                TimeTo = r.TimeTo,
                CompletedDate = r.CompletedDate,
                ClosedDate = r.ClosedDate,
                EstimatedCost = r.EstimatedCost,
                ActualCost = r.ActualCost,
                ResolutionNotes = r.ResolutionNotes,
                Rating = r.Rating,
                TenantFeedback = r.TenantFeedback,
                Images = r.Images?.Select(i => new MaintenanceImageDto
                {
                    ImageId = i.ImageId,
                    RequestId = i.RequestId,
                    ImageUrl = i.ImageUrl,
                    ImagePath = i.ImagePath,
                    Caption = i.Caption,
                    ImageType = i.ImageType,
                    UploadedBy = i.UploadedBy,
                    UploadedByName = i.UploadedByUser?.FullName,
                    UploadedAt = i.UploadedAt
                }).ToList(),
                Comments = r.Comments?.Select(c => new MaintenanceCommentDto
                {
                    CommentId = c.CommentId,
                    RequestId = c.RequestId,
                    UserId = c.UserId,
                    UserName = c.User?.FullName,
                    Comment = c.Comment,
                    IsInternal = c.IsInternal,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToList()
            }).ToList();
        }
    }
}
