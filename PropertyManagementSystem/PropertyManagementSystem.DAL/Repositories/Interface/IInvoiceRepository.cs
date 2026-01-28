using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IInvoiceRepository : IGenericRepository<Invoice>
    {
        Task<List<Invoice>> GetAvailableInvoicesByTenantAsync(int tenantId);
        Task<List<Invoice>> GetInvoicesByLeaseIdAsync(int leaseId);
        Task<Invoice?> UpdateInvoiceAsync(Invoice invoice);
    }
}
