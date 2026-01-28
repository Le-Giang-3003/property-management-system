using PropertyManagementSystem.DAL.Entities;
namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment?> GetPaymentByIdAsync(int paymentId);
        Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId);
        Task<IEnumerable<Payment>> GetPaymentsByTenantIdAsync(int tenantId);
        Task<List<Payment>> GetByTenantAsync(int tenantId);
        Task<List<Payment>> GetByLandlordIdAsync(int landlordId);
        Task<bool> UpdateAsync(Payment payment);

    }
}
