using PropertyManagementSystem.BLL.DTOs.Invoice;
using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.DAL.Entities;
public interface IPaymentService
{
    Task<PaymentDto> MakePaymentAsync(int tenantId, MakePaymentRequestDto request);
    Task<List<InvoiceDto>> GetAvailableInvoicesAsync(int tenantId);
    Task<List<PaymentDto>> GetPaymentHistoryAsync(int tenantId);

}
