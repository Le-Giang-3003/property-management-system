using PropertyManagementSystem.BLL.DTOs.Invoice;
using PropertyManagementSystem.BLL.DTOs.Payments;
public interface IPaymentService
{
    Task<PaymentDto> MakePaymentAsync(int tenantId, MakePaymentRequestDto request);
    Task<List<InvoiceDto>> GetAvailableInvoicesAsync(int tenantId);
    Task<List<PaymentDto>> GetPaymentHistoryAsync(int tenantId);
    Task<PaymentReportDto> GetPaymentReportAsync(int tenantId, DateTime fromDate, DateTime? toDate = null);
}
