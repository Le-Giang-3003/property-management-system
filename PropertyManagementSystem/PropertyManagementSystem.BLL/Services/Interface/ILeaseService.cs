using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface ILeaseService
    {
        Task<Lease?> GetByIdAsync(int leaseId);
        Task<List<Lease>> GetByTenantAsync(int tenantId);
    }
}
