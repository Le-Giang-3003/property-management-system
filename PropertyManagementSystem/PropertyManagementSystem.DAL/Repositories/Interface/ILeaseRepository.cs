using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface ILeaseRepository : IGenericRepository<Lease>
    {
        Task<IEnumerable<Lease>> GetActiveLeasesForTenantAsync(int tenantId);
        Task<Lease?> GetLeaseWithPropertyAsync(int leaseId);
        Task<IEnumerable<Property>> GetTenantActivePropertiesAsync(int tenantId);
        Task<IEnumerable<Lease>> GetLeasesByLandlordAsync(int landlordId);
        Task<bool> HasActiveLease(int tenantId, int propertyId);
    }
}
