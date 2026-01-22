using PropertyManagementSystem.BLL.DTOs.Payments;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IInvoiceService
    {
        Task<List<Invoice>> GetAvailableInvoicesByTenantAsync(int tenantId);
        Task<Invoice?> GetInvoiceByIdAsync(int invoiceId);
    }
}
