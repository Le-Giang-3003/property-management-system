using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class InvoiceRepository : GenericRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Invoice>> GetAvailableInvoicesByTenantAsync(int tenantId)
        {
            return await _context.Invoices
                .Include(i => i.Lease)
                .Where(i => i.Lease.TenantId == tenantId && i.RemainingAmount > 0)
                .ToListAsync();
        }

        public async Task<Invoice?> UpdateInvoiceAsync(Invoice invoice)
        {
            var existingInvoice = await _context.Invoices.FindAsync(invoice.InvoiceId);
            if (existingInvoice == null)
            {
                return null;
            }
            _context.Entry(existingInvoice).CurrentValues.SetValues(invoice);
            await _context.SaveChangesAsync();
            return existingInvoice;
        }
    }
}
