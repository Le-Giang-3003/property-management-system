using PropertyManagementSystem.BLL.DTOs.Invoice;
using PropertyManagementSystem.BLL.DTOs.Payments;

public interface IPaymentService
{
    Task<PaymentDto> MakePaymentAsync(int tenantId, MakePaymentRequestDto request);
    Task<List<InvoiceDto>> GetAvailableInvoicesAsync(int tenantId);
    Task<List<PaymentDto>> GetPaymentHistoryAsync(int tenantId);
    Task<PaymentReportDto> GetPaymentReportAsync(int tenantId, int roleId, DateTime fromDate, DateTime toDate, string? paymentMethod = null);
    Task<bool> RaiseDisputeAsync(int userId, RaiseDisputeDTO dto);
    Task<bool> ProcessRefundAsync(int disputeId, int adminId, string note);
    Task<bool> ResolveDisputeAsync(int adminId, ResolveDisputeDTO dto);
    Task<List<PaymentDisputeDTO>> GetAllDisputesAsync();
}
