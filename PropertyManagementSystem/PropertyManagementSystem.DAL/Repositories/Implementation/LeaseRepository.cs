using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class LeaseRepository : GenericRepository<Lease>, ILeaseRepository
    {
        public LeaseRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Lease>> GetActiveLeasesForTenantAsync(int tenantId)
        {
            return await _dbSet
                .Include(l => l.Property)
                .Where(l => l.TenantId == tenantId && l.Status == "Active")
                .OrderByDescending(l => l.StartDate)
                .ToListAsync();
        }

        public async Task<Lease?> GetLeaseWithPropertyAsync(int leaseId)
        {
            return await _dbSet
                .Include(l => l.Property)
                    .ThenInclude(p => p.Landlord)
                .Include(l => l.Tenant)
                .FirstOrDefaultAsync(l => l.LeaseId == leaseId);
        }

        public async Task<IEnumerable<Property>> GetTenantActivePropertiesAsync(int tenantId)
        {
            return await _dbSet
                .Include(l => l.Property)
                .Where(l => l.TenantId == tenantId && l.Status == "Active")
                .Select(l => l.Property)
                .ToListAsync();
        }

        public async Task<IEnumerable<Lease>> GetLeasesByLandlordAsync(int landlordId)
        {
            return await _dbSet
                .Include(l => l.Property)
                .Include(l => l.Tenant)
                .Where(l => l.Property.LandlordId == landlordId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> HasActiveLease(int tenantId, int propertyId)
        {
            return await _dbSet
                .AnyAsync(l => l.TenantId == tenantId
                            && l.PropertyId == propertyId
                            && l.Status == "Active");
        }
    }
}
