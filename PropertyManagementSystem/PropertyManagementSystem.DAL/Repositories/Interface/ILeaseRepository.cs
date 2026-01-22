using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface ILeaseRepository 
    {
        Task<Lease?> GetByIdAsync(int leaseId);
        Task<List<Lease>> GetByTenantAsync(int tenantId);
        Task AddAsync(Lease lease);
        Task<Lease?> UpdateAsync(Lease lease);
    }
}
