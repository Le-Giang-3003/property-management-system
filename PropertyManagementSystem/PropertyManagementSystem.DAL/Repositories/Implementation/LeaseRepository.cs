    using Microsoft.EntityFrameworkCore;
    using PropertyManagementSystem.DAL.Data;
    using PropertyManagementSystem.DAL.Entities;
    using PropertyManagementSystem.DAL.Repositories.Interface;

    namespace PropertyManagementSystem.DAL.Repositories.Implementation
    {
        public class LeaseRepository : ILeaseRepository
        {
            private readonly AppDbContext _context;

            public LeaseRepository(AppDbContext context)
            {
                _context = context;
            }

            public async Task<Lease?> GetByIdAsync(int leaseId)
            {
                return await _context.Leases
                    .Include(l => l.Property)    // nếu cần
                    .Include(l => l.Tenant)      // nếu cần
                    .FirstOrDefaultAsync(l => l.LeaseId == leaseId);
            }

            public async Task<List<Lease>> GetByTenantAsync(int tenantId)
            {
                return await _context.Leases
                    .Where(l => l.TenantId == tenantId)
                    .ToListAsync();
            }

            public async Task AddAsync(Lease lease)
            {
                _context.Leases.Add(lease);
                await _context.SaveChangesAsync();
            }

            public async Task<Lease?> UpdateAsync(Lease lease)
            {
                var existing = await _context.Leases.FindAsync(lease.LeaseId);
                if (existing == null)
                    return null;

                _context.Entry(existing).CurrentValues.SetValues(lease);
                await _context.SaveChangesAsync();
                return existing;
            }
        }
    }
