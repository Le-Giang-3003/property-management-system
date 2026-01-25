using PropertyManagementSystem.BLL.DTOs.Maintenance;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface ILeaseService
    {
        /// <summary>
        /// Gets properties with active leases for a tenant (for maintenance request dropdown)
        /// </summary>
        Task<List<PropertySelectDto>> GetTenantActivePropertiesAsync(int tenantId);

        /// <summary>
        /// Validates if a tenant has an active lease for a property
        /// </summary>
        Task<bool> ValidateTenantPropertyAccessAsync(int tenantId, int propertyId);
        Task<Lease?> GetByIdAsync(int leaseId);
        Task<List<Lease>> GetByTenantAsync(int tenantId);
    }
}
