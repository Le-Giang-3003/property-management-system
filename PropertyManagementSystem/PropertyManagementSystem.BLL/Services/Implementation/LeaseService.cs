using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class LeaseService : ILeaseService
    {
        private readonly ILeaseRepository _leaseRepository;

        public LeaseService(ILeaseRepository leaseRepository)
        {
            _leaseRepository = leaseRepository;
        }

        public async Task<Lease?> GetByIdAsync(int leaseId)
        {
            return await _leaseRepository.GetByIdAsync(leaseId);
        }

        public async Task<List<Lease>> GetByTenantAsync(int tenantId)
        {
            return await _leaseRepository.GetByTenantAsync(tenantId);
        }
    }
}
