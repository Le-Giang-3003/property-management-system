using PropertyManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface ILeaseRepository
    {
        Task<Lease> CreateAsync(Lease lease);
        Task<Lease> GetByIdAsync(int id);
        Task<IEnumerable<Lease>> GetAllAsync();
        Task<IEnumerable<Lease>> GetByPropertyIdAsync(int propertyId);
        Task<IEnumerable<Lease>> GetByTenantIdAsync(int tenantId);
        Task<IEnumerable<Lease>> GetByLandlordIdAsync(int landlordId);
        Task<Lease> GetByApplicationIdAsync(int applicationId);
        Task<bool> UpdateAsync(Lease lease);
        Task<string> GenerateLeaseNumberAsync();
        Task<IEnumerable<Lease>> GetLeaseHistoryByPropertyIdAsync(int propertyId);
        Task<IEnumerable<Lease>> GetRenewableLeasesAsync(int daysBeforeExpiry = 30);
        Task<Lease?> GetLeaseWithDetailsAsync(int leaseId); 
        Task<IEnumerable<Lease>> GetLeasesByStatusAsync(string status);
    }
}
