using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class PaymentDisputeRepository : IPaymentDisputeRepository
    {
        private readonly AppDbContext _context;

        public PaymentDisputeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PaymentDispute?> GetByIdAsync(int id)
        {
            return await _context.PaymentDisputes
                .Include(d => d.Invoice)
                .FirstOrDefaultAsync(d => d.DisputeId == id);
        }

        public async Task<IEnumerable<PaymentDispute>> GetAllAsync()
        {
            return await _context.PaymentDisputes
                .Include(d => d.Invoice)
                    .ThenInclude(i => i.Lease)
                        .ThenInclude(l => l.Tenant)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentDispute>> GetByRaisedByAsync(int userId)
        {
            return await _context.PaymentDisputes
                .Include(d => d.Invoice)
                .Where(d => d.RaisedBy == userId)
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();
        }

        public async Task AddAsync(PaymentDispute dispute)
        {
            await _context.PaymentDisputes.AddAsync(dispute);
        }

        public async Task UpdateAsync(PaymentDispute dispute)
        {
            dispute.UpdatedAt = DateTime.UtcNow;
            _context.PaymentDisputes.Update(dispute);
            await Task.CompletedTask;
        }
    }
}
