using PropertyManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.DAL.Repositories.Interface.IGenericRepository&lt;PropertyManagementSystem.DAL.Entities.Lease&gt;" />
    public interface ILeaseRepository : IGenericRepository<Lease>
    {
        Task<IEnumerable<Lease>> GetActiveLeasesForTenantAsync(int tenantId);
        Task<Lease?> GetLeaseWithPropertyAsync(int leaseId);
        Task<IEnumerable<Property>> GetTenantActivePropertiesAsync(int tenantId);
        Task<IEnumerable<Lease>> GetLeasesByLandlordAsync(int landlordId);
        Task<bool> HasActiveLease(int tenantId, int propertyId);

        Task<IEnumerable<Lease>> GetLeasesByTenantUserIdAsync(int tenantUserId);

        Task<List<Lease>> GetExpiringByLandlordAsync(int landlordId, int daysAhead = 30);

        // ===== From develop =====
        // CRUD-like methods (kept to avoid breaking existing services)

        Task<Lease> CreateAsync(Lease lease);

        Task<Lease> GetByIdAsync(int id);

        Task<IEnumerable<Lease>> GetAllAsync();

        Task<IEnumerable<Lease>> GetByPropertyIdAsync(int propertyId);

        Task<IEnumerable<Lease>> GetByTenantIdAsync(int tenantId);

        Task<Lease> GetByApplicationIdAsync(int applicationId);

        Task<bool> UpdateAsync(Lease lease);

        // ===== Business / utility methods =====

        Task<string> GenerateLeaseNumberAsync();

        Task<IEnumerable<Lease>> GetLeaseHistoryByPropertyIdAsync(int propertyId);

        Task<IEnumerable<Lease>> GetRenewableLeasesAsync(int daysBeforeExpiry = 30);

        Task<Lease?> GetLeaseWithDetailsAsync(int leaseId);

        Task<IEnumerable<Lease>> GetLeasesByStatusAsync(string status);

        Task<IEnumerable<Lease>> GetByLandlordIdAsync(int landlordId);
    }
}
