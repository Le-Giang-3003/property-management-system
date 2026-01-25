using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface ILeaseRepository : IGenericRepository<Lease>
    public interface ILeaseRepository 
    {
        /// <summary>
        /// Gets active leases for a tenant
        /// </summary>
        Task<IEnumerable<Lease>> GetActiveLeasesForTenantAsync(int tenantId);

        /// <summary>
        /// Gets lease with property details
        /// </summary>
        Task<Lease?> GetLeaseWithPropertyAsync(int leaseId);

        /// <summary>
        /// Gets properties with active leases for a tenant (for maintenance request dropdown)
        /// </summary>
        Task<IEnumerable<Property>> GetTenantActivePropertiesAsync(int tenantId);

        /// <summary>
        /// Gets all leases for a landlord's properties
        /// </summary>
        Task<IEnumerable<Lease>> GetLeasesByLandlordAsync(int landlordId);

        /// <summary>
        /// Checks if a tenant has an active lease for a property
        /// </summary>
        Task<bool> HasActiveLease(int tenantId, int propertyId);
        Task<Lease?> GetByIdAsync(int leaseId);
        Task<List<Lease>> GetByTenantAsync(int tenantId);
        Task AddAsync(Lease lease);
        Task<Lease?> UpdateAsync(Lease lease);
    }
}
