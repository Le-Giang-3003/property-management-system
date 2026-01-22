using PropertyManagementSystem.BLL.DTOs.Schedule;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IPropertyViewingService
    {
        Task<(bool Success, string Message, int? ViewingId)> CreateViewingRequestAsync(CreateViewingRequestDto request, int? tenantId);
        Task<(bool Success, string Message)> ConfirmViewingAsync(ConfirmViewingRequestDto request, int landlordId);
        Task<(bool Success, string Message)> RejectViewingAsync(int viewingId, string? reason, int landlordId);
        Task<(bool Success, string Message)> CancelViewingAsync(int viewingId, int? userId);
        Task<(bool Success, string Message)> RescheduleViewingAsync(RescheduleViewingRequestDto request, int userId, string role);
        Task<(bool Success, string Message)> CompleteViewingAsync(int viewingId, int landlordId);
        Task<(bool Success, string Message)> SubmitFeedbackAsync(ViewingFeedbackRequestDto request, int? userId);
        Task<PropertyViewingDto?> GetViewingByIdAsync(int viewingId);
        Task<IEnumerable<PropertyViewingDto>> GetViewingsByTenantAsync(int tenantId);
        Task<IEnumerable<PropertyViewingDto>> GetViewingsByLandlordAsync(int landlordId);
        Task<IEnumerable<PropertyViewingDto>> GetPendingViewingsAsync(int landlordId);
        Task<IEnumerable<PropertyViewingDto>> GetUpcomingViewingsAsync(int userId, string role);
        Task<IEnumerable<PropertyViewingDto>> GetViewingsByPropertyAsync(int propertyId);
        Task<bool> CheckTimeSlotAvailabilityAsync(int propertyId, DateTime scheduledDate, int? excludeViewingId = null);
    }
}
