using PropertyManagementSystem.BLL.DTOs.Lease;
using PropertyManagementSystem.BLL.DTOs.Maintenance;
using PropertyManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyManagementSystem.DAL.Entities;  

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface ILeaseService
    {
        Task<List<PropertySelectDto>> GetTenantActivePropertiesAsync(int tenantId);
        Task<bool> ValidateTenantPropertyAccessAsync(int tenantId, int propertyId);

        Task<Lease?> GetByIdAsync(int leaseId);
        Task<List<Lease>> GetByTenantAsync(int tenantId);
    }
}
