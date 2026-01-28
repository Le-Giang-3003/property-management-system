using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IMaintenanceRepository
    {
        // Create
        Task<MaintenanceRequest> CreateAsync(MaintenanceRequest request);
        Task<MaintenanceImage> AddImageAsync(MaintenanceImage image);
        Task<MaintenanceComment> AddCommentAsync(MaintenanceComment comment);

        // Read
        Task<MaintenanceRequest> GetByIdAsync(int requestId);
        Task<List<MaintenanceRequest>> GetAllAsync();
        Task<List<MaintenanceRequest>> GetByTenantIdAsync(int tenantId);
        Task<List<MaintenanceRequest>> GetByLandlordIdAsync(int landlordId);
        Task<List<MaintenanceRequest>> GetByTechnicianIdAsync(int technicianId);
        Task<List<MaintenanceRequest>> GetByPropertyIdAsync(int propertyId);
        Task<List<MaintenanceRequest>> GetByStatusAsync(string status);
        Task<string> GetLatestRequestNumberAsync();

        // Update
        Task<MaintenanceRequest> UpdateAsync(MaintenanceRequest request);
        Task<bool> AssignTechnicianAsync(int requestId, int technicianId);
        Task<bool> UpdateStatusAsync(int requestId, string status);
        Task<bool> CompleteMaintenanceAsync(int requestId, decimal actualCost, string resolutionNotes);
        Task<bool> RateMaintenanceAsync(int requestId, int rating, string feedback);
        Task<bool> CancelRequestAsync(int requestId);

        // Delete
        Task<bool> DeleteAsync(int requestId);

        // Statistics
        Task<Dictionary<string, int>> GetRequestStatsByTenantAsync(int tenantId);
        Task<Dictionary<string, int>> GetRequestStatsByLandlordAsync(int landlordId);
        Task<Dictionary<string, int>> GetRequestStatsByTechnicianAsync(int technicianId);

        Task<int> GetPendingCountByLandlordAsync(int landlordId);
        Task<List<MaintenanceRequest>> GetRecentByLandlordAsync(int landlordId, int take = 5);
    }
}
