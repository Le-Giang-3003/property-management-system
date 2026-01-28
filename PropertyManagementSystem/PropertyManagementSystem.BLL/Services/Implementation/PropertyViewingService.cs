using PropertyManagementSystem.BLL.DTOs.Schedule;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class PropertyViewingService : IPropertyViewingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PropertyViewingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        #region Create/Update Operations

        public async Task<(bool Success, string Message, int? ViewingId)> CreateViewingRequestAsync(CreateViewingRequestDto request, int? tenantId)
        {
            // Validate property exists and is available
            var property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId);
            if (property == null)
                return (false, "Property not found.", null);

            if (property.Status != "Available")
                return (false, "Property is not available for viewing.", null);

            // Guest validation
            if (!tenantId.HasValue)
            {
                if (string.IsNullOrEmpty(request.GuestName) ||
                    string.IsNullOrEmpty(request.GuestEmail) ||
                    string.IsNullOrEmpty(request.GuestPhone))
                {
                    return (false, "Guest information (name, email, phone) is required.", null);
                }
            }

            // Validate preferred dates are in future
            if (request.PreferredDate1 <= DateTime.UtcNow)
                return (false, "Preferred date must be in the future.", null);

            var viewing = new PropertyViewing
            {
                PropertyId = request.PropertyId,
                RequestedBy = tenantId,
                GuestName = request.GuestName,
                GuestEmail = request.GuestEmail,
                GuestPhone = request.GuestPhone,
                RequestedDate = DateTime.UtcNow,
                PreferredDate1 = request.PreferredDate1,
                PreferredDate2 = request.PreferredDate2,
                PreferredDate3 = request.PreferredDate3,
                TenantNotes = request.TenantNotes,
                IsVirtualViewing = request.IsVirtualViewing,
                Status = "Requested",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PropertyViewings.AddAsync(viewing);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Viewing request submitted successfully.", viewing.ViewingId);
        }

        public async Task<(bool Success, string Message)> ConfirmViewingAsync(ConfirmViewingRequestDto request, int landlordId)
        {
            var viewing = await _unitOfWork.PropertyViewings.GetViewingWithDetailsAsync(request.ViewingId);

            if (viewing == null)
                return (false, "Viewing request not found.");

            if (viewing.Property.LandlordId != landlordId)
                return (false, "You are not authorized to confirm this viewing.");

            if (viewing.Status != "Requested" && viewing.Status != "Rescheduled")
                return (false, $"Cannot confirm viewing with status '{viewing.Status}'.");

            // Check for conflicts
            if (await _unitOfWork.PropertyViewings.HasConflictingViewingAsync(
                viewing.PropertyId, request.ScheduledDate, viewing.ViewingId))
            {
                return (false, "There is a conflicting viewing scheduled around this time.");
            }

            viewing.ScheduledDate = request.ScheduledDate;
            viewing.LandlordNotes = request.LandlordNotes;
            viewing.MeetingLink = request.MeetingLink;
            viewing.Status = "Confirmed";
            viewing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PropertyViewings.Update(viewing);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Viewing confirmed successfully.");
        }

        public async Task<(bool Success, string Message)> RejectViewingAsync(int viewingId, string? reason, int landlordId)
        {
            var viewing = await _unitOfWork.PropertyViewings.GetViewingWithDetailsAsync(viewingId);

            if (viewing == null)
                return (false, "Viewing request not found.");

            if (viewing.Property.LandlordId != landlordId)
                return (false, "You are not authorized to reject this viewing.");

            if (viewing.Status != "Requested")
                return (false, $"Cannot reject viewing with status '{viewing.Status}'.");

            viewing.Status = "Rejected";
            viewing.LandlordNotes = reason;
            viewing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PropertyViewings.Update(viewing);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Viewing request rejected.");
        }

        public async Task<(bool Success, string Message)> CancelViewingAsync(int viewingId, int? userId)
        {
            var viewing = await _unitOfWork.PropertyViewings.GetByIdAsync(viewingId);

            if (viewing == null)
                return (false, "Viewing request not found.");

            // Only requester can cancel (tenant or guest via viewingId check)
            if (userId.HasValue && viewing.RequestedBy != userId)
                return (false, "You are not authorized to cancel this viewing.");

            if (viewing.Status == "Completed" || viewing.Status == "Cancelled")
                return (false, $"Cannot cancel viewing with status '{viewing.Status}'.");

            viewing.Status = "Cancelled";
            viewing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PropertyViewings.Update(viewing);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Viewing cancelled successfully.");
        }

        public async Task<(bool Success, string Message)> RescheduleViewingAsync(RescheduleViewingRequestDto request, int userId, string role)
        {
            var viewing = await _unitOfWork.PropertyViewings.GetViewingWithDetailsAsync(request.ViewingId);

            if (viewing == null)
                return (false, "Viewing request not found.");

            // Authorization check
            bool authorized = role switch
            {
                "Landlord" => viewing.Property.LandlordId == userId,
                "Tenant" => viewing.RequestedBy == userId,
                _ => false
            };

            if (!authorized)
                return (false, "You are not authorized to reschedule this viewing.");

            if (viewing.Status != "Confirmed" && viewing.Status != "Requested")
                return (false, $"Cannot reschedule viewing with status '{viewing.Status}'.");

            // Check for conflicts
            if (await _unitOfWork.PropertyViewings.HasConflictingViewingAsync(
                viewing.PropertyId, request.NewScheduledDate, viewing.ViewingId))
            {
                return (false, "There is a conflicting viewing scheduled around this time.");
            }

            viewing.ScheduledDate = request.NewScheduledDate;
            viewing.Status = "Rescheduled";

            if (role == "Landlord")
                viewing.LandlordNotes = request.Notes;
            else
                viewing.TenantNotes = request.Notes;

            viewing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PropertyViewings.Update(viewing);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Viewing rescheduled successfully.");
        }

        public async Task<(bool Success, string Message)> CompleteViewingAsync(int viewingId, int landlordId)
        {
            var viewing = await _unitOfWork.PropertyViewings.GetViewingWithDetailsAsync(viewingId);

            if (viewing == null)
                return (false, "Viewing not found.");

            if (viewing.Property.LandlordId != landlordId)
                return (false, "You are not authorized to complete this viewing.");

            if (viewing.Status != "Confirmed")
                return (false, $"Cannot complete viewing with status '{viewing.Status}'.");

            viewing.Status = "Completed";
            viewing.CompletedDate = DateTime.UtcNow;
            viewing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PropertyViewings.Update(viewing);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Viewing marked as completed.");
        }

        public async Task<(bool Success, string Message)> SubmitFeedbackAsync(ViewingFeedbackRequestDto request, int? userId)
        {
            var viewing = await _unitOfWork.PropertyViewings.GetByIdAsync(request.ViewingId);

            if (viewing == null)
                return (false, "Viewing not found.");

            // Only requester can submit feedback
            if (userId.HasValue && viewing.RequestedBy != userId)
                return (false, "You are not authorized to submit feedback for this viewing.");

            if (viewing.Status != "Completed")
                return (false, "Can only submit feedback for completed viewings.");

            if (viewing.Rating.HasValue)
                return (false, "Feedback has already been submitted.");

            viewing.Rating = request.Rating;
            viewing.Feedback = request.Feedback;
            viewing.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PropertyViewings.Update(viewing);
            await _unitOfWork.SaveChangesAsync();

            return (true, "Feedback submitted successfully.");
        }

        #endregion

        #region Get Operations

        public async Task<PropertyViewingDto?> GetViewingByIdAsync(int viewingId)
        {
            var viewing = await _unitOfWork.PropertyViewings.GetViewingWithDetailsAsync(viewingId);
            return viewing == null ? null : MapToDto(viewing);
        }

        public async Task<IEnumerable<PropertyViewingDto>> GetViewingsByTenantAsync(int tenantId)
        {
            var viewings = await _unitOfWork.PropertyViewings.GetByTenantIdAsync(tenantId);
            return viewings.Select(MapToDto);
        }

        public async Task<IEnumerable<PropertyViewingDto>> GetViewingsByLandlordAsync(int landlordId)
        {
            var viewings = await _unitOfWork.PropertyViewings.GetByLandlordIdAsync(landlordId);
            return viewings.Select(MapToDto);
        }

        public async Task<IEnumerable<PropertyViewingDto>> GetPendingViewingsAsync(int landlordId)
        {
            var viewings = await _unitOfWork.PropertyViewings.GetPendingViewingsAsync(landlordId);
            return viewings.Select(MapToDto);
        }

        public async Task<IEnumerable<PropertyViewingDto>> GetUpcomingViewingsAsync(int userId, string role)
        {
            var viewings = await _unitOfWork.PropertyViewings.GetUpcomingViewingsAsync(userId, role);
            return viewings.Select(MapToDto);
        }

        public async Task<IEnumerable<PropertyViewingDto>> GetViewingsByPropertyAsync(int propertyId)
        {
            var viewings = await _unitOfWork.PropertyViewings.GetByPropertyIdAsync(propertyId);
            return viewings.Select(MapToDto);
        }

        public async Task<bool> CheckTimeSlotAvailabilityAsync(int propertyId, DateTime scheduledDate, int? excludeViewingId = null)
        {
            return !await _unitOfWork.PropertyViewings.HasConflictingViewingAsync(propertyId, scheduledDate, excludeViewingId);
        }
        public async Task<PagedViewingResultDto> GetViewingHistoryAsync(
            int? userId,
            string? role,
            ViewingHistoryFilterDto filter)
        {
            var (items, totalCount) = await _unitOfWork.PropertyViewings.GetViewingHistoryAsync(
                userId,
                role,
                filter.Status,
                filter.PropertyId,
                filter.FromDate,
                filter.ToDate,
                filter.SearchTerm,
                filter.PageNumber,
                filter.PageSize);

            return new PagedViewingResultDto
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
        public async Task<PagedViewingResultDto> GetAllViewingsAsync(ViewingHistoryFilterDto filter)
        {
            var (items, totalCount) = await _unitOfWork.PropertyViewings.GetAllViewingsAsync(
                filter.Status,
                filter.PropertyId,
                filter.FromDate,
                filter.ToDate,
                filter.SearchTerm,
                filter.PageNumber,
                filter.PageSize);

            return new PagedViewingResultDto
            {
                Items = items.Select(MapToDto),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        #endregion

        #region Private Methods

        private static PropertyViewingDto MapToDto(PropertyViewing viewing)
        {
            return new PropertyViewingDto
            {
                ViewingId = viewing.ViewingId,
                PropertyId = viewing.PropertyId,
                PropertyName = viewing.Property?.Name ?? string.Empty,
                PropertyAddress = viewing.Property?.Address ?? string.Empty,
                PropertyThumbnail = viewing.Property?.Images?.FirstOrDefault(i => i.IsThumbnail)?.ImageUrl,

                RequestedBy = viewing.RequestedBy,
                TenantName = viewing.Tenant?.FullName,
                TenantEmail = viewing.Tenant?.Email,
                TenantPhone = viewing.Tenant?.PhoneNumber,

                GuestName = viewing.GuestName,
                GuestEmail = viewing.GuestEmail,
                GuestPhone = viewing.GuestPhone,

                LandlordName = viewing.Property?.Landlord?.FullName ?? string.Empty,
                LandlordEmail = viewing.Property?.Landlord?.Email ?? string.Empty,
                LandlordPhone = viewing.Property?.Landlord?.PhoneNumber ?? string.Empty,

                RequestedDate = viewing.RequestedDate,
                PreferredDate1 = viewing.PreferredDate1,
                PreferredDate2 = viewing.PreferredDate2,
                PreferredDate3 = viewing.PreferredDate3,
                ScheduledDate = viewing.ScheduledDate,

                TenantNotes = viewing.TenantNotes,
                LandlordNotes = viewing.LandlordNotes,
                MeetingLink = viewing.MeetingLink,

                Status = viewing.Status,
                IsVirtualViewing = viewing.IsVirtualViewing,

                Rating = viewing.Rating,
                Feedback = viewing.Feedback,

                CreatedAt = viewing.CreatedAt
            };
        }

        #endregion
    }
}
