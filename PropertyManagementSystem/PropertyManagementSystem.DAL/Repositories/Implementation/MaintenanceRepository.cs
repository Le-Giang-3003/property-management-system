using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class MaintenanceRepository : IMaintenanceRepository
    {
        private readonly AppDbContext _context;

        public MaintenanceRepository(AppDbContext context)
        {
            _context = context;
        }

        // Create
        public async Task<MaintenanceRequest> CreateAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<MaintenanceImage> AddImageAsync(MaintenanceImage image)
        {
            _context.MaintenanceImages.Add(image);
            await _context.SaveChangesAsync();
            return image;
        }

        public async Task<MaintenanceComment> AddCommentAsync(MaintenanceComment comment)
        {
            _context.MaintenanceComments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        // Read
        public async Task<MaintenanceRequest> GetByIdAsync(int requestId)
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Property)
                .Include(mr => mr.Tenant)
                .Include(mr => mr.Technician)
                .Include(mr => mr.Images)
                    .ThenInclude(i => i.UploadedByUser)
                .Include(mr => mr.Comments)
                    .ThenInclude(c => c.User)
                .FirstOrDefaultAsync(mr => mr.RequestId == requestId);
        }

        public async Task<List<MaintenanceRequest>> GetAllAsync()
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Property)
                .Include(mr => mr.Tenant)
                .Include(mr => mr.Technician)
                .Include(mr => mr.Images)
                .Include(mr => mr.Comments)
                .OrderByDescending(mr => mr.RequestDate)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetByTenantIdAsync(int tenantId)
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Property)
                .Include(mr => mr.Tenant)
                .Include(mr => mr.Technician)
                .Include(mr => mr.Images)
                .Include(mr => mr.Comments)
                .Where(mr => mr.RequestedBy == tenantId)
                .OrderByDescending(mr => mr.RequestDate)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetByLandlordIdAsync(int landlordId)
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Property)
                .Include(mr => mr.Tenant)
                .Include(mr => mr.Technician)
                .Include(mr => mr.Images)
                .Include(mr => mr.Comments)
                .Where(mr => mr.Property.LandlordId == landlordId)
                .OrderByDescending(mr => mr.RequestDate)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetByTechnicianIdAsync(int technicianId)
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Property)
                .Include(mr => mr.Tenant)
                .Include(mr => mr.Technician)
                .Include(mr => mr.Images)
                .Include(mr => mr.Comments)
                .Where(mr => mr.AssignedTo == technicianId)
                .OrderByDescending(mr => mr.RequestDate)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Property)
                .Include(mr => mr.Tenant)
                .Include(mr => mr.Technician)
                .Include(mr => mr.Images)
                .Include(mr => mr.Comments)
                .Where(mr => mr.PropertyId == propertyId)
                .OrderByDescending(mr => mr.RequestDate)
                .ToListAsync();
        }

        public async Task<List<MaintenanceRequest>> GetByStatusAsync(string status)
        {
            return await _context.MaintenanceRequests
                .Include(mr => mr.Property)
                .Include(mr => mr.Tenant)
                .Include(mr => mr.Technician)
                .Include(mr => mr.Images)
                .Include(mr => mr.Comments)
                .Where(mr => mr.Status == status)
                .OrderByDescending(mr => mr.RequestDate)
                .ToListAsync();
        }

        public async Task<string> GetLatestRequestNumberAsync()
        {
            var latestRequest = await _context.MaintenanceRequests
                .OrderByDescending(mr => mr.RequestId)
                .FirstOrDefaultAsync();

            return latestRequest?.RequestNumber;
        }

        // Update
        public async Task<MaintenanceRequest> UpdateAsync(MaintenanceRequest request)
        {
            _context.MaintenanceRequests.Update(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<bool> AssignTechnicianAsync(int requestId, int technicianId)
        {
            var request = await _context.MaintenanceRequests.FindAsync(requestId);
            if (request == null) return false;

            request.AssignedTo = technicianId;
            request.AssignedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            request.Status = "Assigned";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int requestId, string status)
        {
            var request = await _context.MaintenanceRequests.FindAsync(requestId);
            if (request == null) return false;

            request.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CompleteMaintenanceAsync(int requestId, decimal actualCost, string resolutionNotes)
        {
            var request = await _context.MaintenanceRequests.FindAsync(requestId);
            if (request == null) return false;

            request.ActualCost = actualCost;
            request.ResolutionNotes = resolutionNotes;
            request.CompletedDate = DateTime.UtcNow;
            request.TechnicianStatus = "Completed";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RateMaintenanceAsync(int requestId, int rating, string feedback)
        {
            var request = await _context.MaintenanceRequests.FindAsync(requestId);
            if (request == null) return false;

            request.Rating = rating;
            request.TenantFeedback = feedback;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelRequestAsync(int requestId)
        {
            var request = await _context.MaintenanceRequests.FindAsync(requestId);
            if (request == null) return false;

            if(request.AssignedTo != null)
            {
                request.Status = "Cancelled";
                request.TechnicianStatus = "TenantCancelled";
            }
            else
            {
                request.Status = "Cancelled";
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // Delete
        public async Task<bool> DeleteAsync(int requestId)
        {
            var request = await _context.MaintenanceRequests.FindAsync(requestId);
            if (request == null) return false;

            _context.MaintenanceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        // Statistics
        public async Task<Dictionary<string, int>> GetRequestStatsByTenantAsync(int tenantId)
        {
            var requests = await _context.MaintenanceRequests
                .Where(mr => mr.RequestedBy == tenantId)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", requests.Count },
                { "Pending", requests.Count(r => r.Status == "Pending") },
                { "Assigned", requests.Count(r => r.Status == "Assigned") },
                { "InProgress", requests.Count(r => r.Status == "InProgress") },
                { "Completed", requests.Count(r => r.Status == "Completed") },
                { "Cancelled", requests.Count(r => r.Status == "Cancelled") }
            };
        }

        public async Task<Dictionary<string, int>> GetRequestStatsByLandlordAsync(int landlordId)
        {
            var requests = await _context.MaintenanceRequests
                .Include(mr => mr.Property)
                .Where(mr => mr.Property.LandlordId == landlordId)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", requests.Count },
                { "Pending", requests.Count(r => r.Status == "Pending") },
                { "Assigned", requests.Count(r => r.Status == "Assigned") },
                { "InProgress", requests.Count(r => r.Status == "InProgress") },
                { "Completed", requests.Count(r => r.Status == "Completed") },
                { "Cancelled", requests.Count(r => r.Status == "Cancelled") }
            };
        }

        public async Task<Dictionary<string, int>> GetRequestStatsByTechnicianAsync(int technicianId)
        {
            var requests = await _context.MaintenanceRequests
                .Where(mr => mr.AssignedTo == technicianId)
                .ToListAsync();

            return new Dictionary<string, int>
            {
                { "Total", requests.Count },
                { "Pending", requests.Count(r => r.TechnicianStatus == null) },
                { "Accepted", requests.Count(r => r.TechnicianStatus == "Accepted") },
                { "InProgress", requests.Count(r => r.Status == "InProgress") },
                { "Completed", requests.Count(r => r.Status == "Completed") }
            };
        }

        public async Task<int> GetPendingCountByLandlordAsync(int landlordId)
        {
            return await _context.MaintenanceRequests
                .Include(m => m.Property)
                .CountAsync(m => m.Property.LandlordId == landlordId && m.Status == "Pending");
        }

        public async Task<List<MaintenanceRequest>> GetRecentByLandlordAsync(int landlordId, int take = 5)
        {
            return await _context.MaintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Tenant)
                .Include(m => m.Technician)
                .Where(m => m.Property.LandlordId == landlordId)
                .OrderByDescending(m => m.RequestDate)
                .Take(take)
                .ToListAsync();
        }
    }
}
