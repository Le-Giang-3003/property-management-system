using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Interface.IGenericRepository&lt;PropertyManagementSystem.DAL.Entities.Lease&gt;" />
    public interface ILeaseRepository : IGenericRepository<Lease>
    {
        /// <summary>
        /// Gets active leases for a tenant
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<Lease>> GetActiveLeasesForTenantAsync(int tenantId);

        /// <summary>
        /// Gets lease with property details
        /// </summary>
        /// <param name="leaseId">The lease identifier.</param>
        /// <returns></returns>
        Task<Lease?> GetLeaseWithPropertyAsync(int leaseId);

        /// <summary>
        /// Gets properties with active leases for a tenant (for maintenance request dropdown)
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<Property>> GetTenantActivePropertiesAsync(int tenantId);

        /// <summary>
        /// Gets all leases for a landlord's properties
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<Lease>> GetLeasesByLandlordAsync(int landlordId);

        /// <summary>
        /// Checks if a tenant has an active lease for a property
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="propertyId">The property identifier.</param>
        /// <returns></returns>
        Task<bool> HasActiveLease(int tenantId, int propertyId);
        /// <summary>
        /// Gets the leases by tenant user identifier asynchronous.
        /// </summary>
        /// <param name="tenantUserId">The tenant user identifier.</param>
        /// <returns></returns>
        Task<IEnumerable<Lease>> GetLeasesByTenantUserIdAsync(int tenantUserId);
        /// <summary>
        /// Gets the expiring by landlord asynchronous.
        /// </summary>
        /// <param name="landlordId">The landlord identifier.</param>
        /// <param name="daysAhead">The days ahead.</param>
        /// <returns></returns>
        Task<List<Lease>> GetExpiringByLandlordAsync(int landlordId, int daysAhead = 30);
    }
}
