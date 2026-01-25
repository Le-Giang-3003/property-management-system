using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task AddAsync(Payment payment)
        {
            await _dbSet.AddAsync(payment);
            await _context.SaveChangesAsync();
        }
        public async Task<Payment?> GetPaymentByIdAsync(int paymentId)
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(int invoiceId)
        {
            return await _context.Payments
                .Where(p => p.InvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByTenantIdAsync(int tenantId)
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                .ThenInclude(i => i.Lease)
                .Where(p => p.Invoice.Lease.TenantId == tenantId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
        public async Task<List<Payment>> GetByTenantAsync(int tenantId)
        {
            return await _context.Payments
                .Include(p => p.Invoice)
                    .ThenInclude(i => i.Lease)
                .Where(p => p.Invoice.Lease.TenantId == tenantId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

    }
}
