using PropertyManagementSystem.BLL.DTOs.Maintenance;
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

        public async Task<List<PropertySelectDto>> GetTenantActivePropertiesAsync(int tenantId)
        {
            var properties = await _leaseRepository.GetTenantActivePropertiesAsync(tenantId);

            return properties.Select(p => new PropertySelectDto
            {
                PropertyId = p.PropertyId,
                Name = p.Name,
                Address = p.Address
            }).ToList();
        }

        public async Task<bool> ValidateTenantPropertyAccessAsync(int tenantId, int propertyId)
        {
            return await _leaseRepository.HasActiveLease(tenantId, propertyId);
        }
    }
}
