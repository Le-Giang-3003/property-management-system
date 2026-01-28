using PropertyManagementSystem.BLL.DTOs.Invoice;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IInvoiceService
    {
        Task<List<InvoiceDto>> GetAvailableInvoicesByTenantAsync(int tenantId);
        Task<InvoiceDto?> GetInvoiceByIdAsync(int invoiceId);
        Task<InvoiceDto?> UpdateInvoiceAsync(InvoiceDto invoice);
        Task<InvoiceDto> CreateInvoiceFromLeaseAsync(int leaseId, DateTime periodStart, DateTime periodEnd);
        Task<List<ActiveLeaseForInvoiceDto>> GetActiveLeasesByLandlordIdAsync(int landlordId);
        Task<InvoiceDto> CreateInvoiceWithAdditionalAmountAsync(int leaseId, decimal additionalAmount, string? additionalDescription);
    }
}
