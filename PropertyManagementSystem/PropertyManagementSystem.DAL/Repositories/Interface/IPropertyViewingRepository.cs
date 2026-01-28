using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IPropertyViewingRepository : IGenericRepository<PropertyViewing>
    {
        Task<IEnumerable<PropertyViewing>> GetByPropertyIdAsync(int propertyId);
        Task<IEnumerable<PropertyViewing>> GetByTenantIdAsync(int tenantId);
        Task<IEnumerable<PropertyViewing>> GetByLandlordIdAsync(int landlordId);
        Task<IEnumerable<PropertyViewing>> GetPendingViewingsAsync(int landlordId);
        Task<IEnumerable<PropertyViewing>> GetUpcomingViewingsAsync(int userId, string role);
        Task<PropertyViewing?> GetViewingWithDetailsAsync(int viewingId);
        Task<bool> HasConflictingViewingAsync(int propertyId, DateTime scheduledDate, int? excludeViewingId = null);
        Task<(IEnumerable<PropertyViewing> Items, int TotalCount)> GetViewingHistoryAsync(
            int? userId,
            string? role,
            string? status,
            int? propertyId,
            DateTime? fromDate,
            DateTime? toDate,
            string? searchTerm,
            int pageNumber,
            int pageSize);
        Task<(IEnumerable<PropertyViewing> Items, int TotalCount)> GetAllViewingsAsync(
            string? status,
            int? propertyId,
            DateTime? fromDate,
            DateTime? toDate,
            string? searchTerm,
            int pageNumber,
            int pageSize);
    }
}
