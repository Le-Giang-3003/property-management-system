using PropertyManagementSystem.BLL.DTOs.Maintenance;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IMaintenanceService
    {
        // Tenant Operations
        Task<MaintenanceRequestDto> CreateRequestAsync(CreateMaintenanceRequestDto dto, int tenantId);
        Task<bool> CancelRequestAsync(int requestId, int tenantId);
        Task<bool> RateMaintenanceAsync(RateMaintenanceDto dto, int tenantId);
        Task<List<MaintenanceRequestDto>> GetTenantRequestsAsync(int tenantId, string status = null);
        Task<MaintenanceStatsDto> GetTenantStatsAsync(int tenantId);

        // Landlord Operations
        Task<List<MaintenanceRequestDto>> GetLandlordRequestsAsync(int landlordId, string status = null);
        Task<bool> RejectRequestAsync(RejectMaintenanceRequestDto dto, int landlordId);
        Task<bool> AssignTechnicianAsync(AssignTechnicianDto dto, int landlordId);
        Task<bool> CloseRequestAsync(int requestId, int landlordId);
        Task<bool> ConfirmCompletionAsync(int requestId, int landlordId);
        Task<MaintenanceStatsDto> GetLandlordStatsAsync(int landlordId);
        Task<List<UserDto>> GetAvailableTechniciansAsync();

        // Technician Operations
        Task<List<MaintenanceRequestDto>> GetTechnicianRequestsAsync(int technicianId, string status = null);
        Task<bool> RespondToAssignmentAsync(TechnicianResponseDto dto, int technicianId);
        Task<bool> CompleteMaintenanceAsync(CompleteMaintenanceDto dto, int technicianId);
        Task<MaintenanceStatsDto> GetTechnicianStatsAsync(int technicianId);

        // Common Operations
        Task<MaintenanceRequestDto> GetRequestByIdAsync(int requestId);
        Task<bool> AddCommentAsync(int requestId, int userId, string comment);
    }

    public class UserDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
