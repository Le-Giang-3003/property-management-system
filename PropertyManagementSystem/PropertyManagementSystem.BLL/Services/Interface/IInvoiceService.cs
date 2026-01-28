using PropertyManagementSystem.BLL.DTOs.Invoice;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IInvoiceService
    {
        Task<List<InvoiceDto>> GetAvailableInvoicesByTenantAsync(int tenantId);
        Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId);
        Task<InvoiceDto?> UpdateInvoiceAsync(InvoiceDto invoice);
        Task<InvoiceDto> CreateInvoiceFromLeaseAsync(int leaseId, DateTime periodStart, DateTime periodEnd);
        Task<InvoiceDto?> CreateFirstInvoiceWithDepositAsync(int leaseId);
        Task<List<ActiveLeaseForInvoiceDto>> GetActiveLeasesByLandlordIdAsync(int landlordId);
        Task<InvoiceDto> CreateInvoiceWithAdditionalAmountAsync(int leaseId, decimal additionalAmount, string? additionalDescription);

        // New methods for automatic invoice generation
        Task<InvoiceDto?> CreateMonthlyInvoiceAsync(Lease lease, DateTime billingMonth);
        Task<bool> HasInvoiceForMonthAsync(int leaseId, DateTime billingMonth);
        Task UpdateOverdueInvoicesAsync();
    }
}
