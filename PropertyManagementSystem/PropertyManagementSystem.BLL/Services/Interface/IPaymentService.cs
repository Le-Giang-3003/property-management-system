using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IPaymentService
    {
        Task<PaymentDto> MakePaymentAsync(int tenantId, MakePaymentRequestDto request);
        Task<List<Invoice>> GetAvailableInvoicesAsync(int tenantId);
    }
}
