using PropertyManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface ILeaseSignatureRepository
    {
        Task<LeaseSignature> CreateAsync(LeaseSignature signature);
        Task<IEnumerable<LeaseSignature>> GetByLeaseIdAsync(int leaseId);
        Task<LeaseSignature?> GetByLeaseAndUserAsync(int leaseId, int userId);
        Task<bool> HasLandlordSignedAsync(int leaseId);
        Task<bool> HasTenantSignedAsync(int leaseId);
        Task<bool> IsFullySignedAsync(int leaseId);
    }
}
