using PropertyManagementSystem.BLL.DTOs.Lease;
using PropertyManagementSystem.BLL.DTOs.Maintenance;
using PropertyManagementSystem.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface ILeaseService
    {
        Task<Lease> GetLeaseByIdAsync(int id);
        Task<IEnumerable<Lease>> GetAllLeasesAsync();
        Task<IEnumerable<Lease>> GetLeasesByPropertyIdAsync(int propertyId);
        Task<IEnumerable<Lease>> GetLeasesByTenantIdAsync(int tenantId);
        Task<IEnumerable<Lease>> GetLeasesByLandlordIdAsync(int landlordId);
        Task<Lease> CreateLeaseFromApplicationAsync(CreateLeaseDto dto, int createdBy);
        Task<bool> UpdateLeaseAsync(UpdateLeaseDto dto);
        Task<Lease?> GetLeaseByApplicationIdAsync(int applicationId);
        Task<IEnumerable<Lease>> GetLeaseHistoryByPropertyIdAsync(int propertyId);
        Task<bool> CanCreateLeaseFromApplication(int applicationId);
        //Signature
        Task<SignLeaseResponseDto> SignLeaseAsync(SignLeaseDto dto);
        Task<bool> IsLeaseFullySignedAsync(int leaseId);
        Task<bool> CanUserSignAsync(int leaseId, int userId);
        Task<IEnumerable<LeaseSignatureDto>> GetLeaseSignaturesAsync(int leaseId);
        Task UpdateExpiredLeasesAsync();
        Task<bool> TerminateLeaseAsync(Lease lease, TerminateLeaseDto terminateDto);
        Task<RenewLeaseResponseDto> RenewLeaseAsync(RenewLeaseDto dto, int renewedBy);
        Task<bool> CanRenewLeaseAsync(int leaseId);
        Task<IEnumerable<Lease>> GetRenewableLeasesAsync();  // Lease sắp hết hạn
        Task<List<PropertySelectDto>> GetTenantActivePropertiesAsync(int tenantId);
    }
}
