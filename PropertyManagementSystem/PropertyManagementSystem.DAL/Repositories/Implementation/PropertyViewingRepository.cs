using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class PropertyViewingRepository : GenericRepository<PropertyViewing>, IPropertyViewingRepository
    {
        public PropertyViewingRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<PropertyViewing>> GetByPropertyIdAsync(int propertyId)
        {
            return await _dbSet
                .Include(v => v.Property)
                .Include(v => v.Tenant)
                .Where(v => v.PropertyId == propertyId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyViewing>> GetByTenantIdAsync(int tenantId)
        {
            return await _dbSet
                .Include(v => v.Property)
                    .ThenInclude(p => p.Landlord)
                .Include(v => v.Property)
                    .ThenInclude(p => p.Images)
                .Where(v => v.RequestedBy == tenantId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyViewing>> GetByLandlordIdAsync(int landlordId)
        {
            return await _dbSet
                .Include(v => v.Property)
                .Include(v => v.Tenant)
                .Where(v => v.Property.LandlordId == landlordId)
                .OrderByDescending(v => v.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyViewing>> GetPendingViewingsAsync(int landlordId)
        {
            return await _dbSet
                .Include(v => v.Property)
                .Include(v => v.Tenant)
                .Where(v => v.Property.LandlordId == landlordId && v.Status == "Requested")
                .OrderBy(v => v.RequestedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<PropertyViewing>> GetUpcomingViewingsAsync(int userId, string role)
        {
            var query = _dbSet
                .Include(v => v.Property)
                    .ThenInclude(p => p.Landlord)
                .Include(v => v.Tenant)
                .Where(v => v.Status == "Confirmed" && v.ScheduledDate > DateTime.UtcNow);

            if (role == "Tenant")
            {
                query = query.Where(v => v.RequestedBy == userId);
            }
            else if (role == "Landlord")
            {
                query = query.Where(v => v.Property.LandlordId == userId);
            }

            return await query.OrderBy(v => v.ScheduledDate).ToListAsync();
        }

        public async Task<PropertyViewing?> GetViewingWithDetailsAsync(int viewingId)
        {
            return await _dbSet
                .Include(v => v.Property)
                    .ThenInclude(p => p.Landlord)
                .Include(v => v.Property)
                    .ThenInclude(p => p.Images)
                .Include(v => v.Tenant)
                .FirstOrDefaultAsync(v => v.ViewingId == viewingId);
        }

        public async Task<bool> HasConflictingViewingAsync(int propertyId, DateTime scheduledDate, int? excludeViewingId = null)
        {
            var startTime = scheduledDate.AddMinutes(-30);
            var endTime = scheduledDate.AddMinutes(30);

            var query = _dbSet.Where(v =>
                v.PropertyId == propertyId &&
                v.Status == "Confirmed" &&
                v.ScheduledDate >= startTime &&
                v.ScheduledDate <= endTime);

            if (excludeViewingId.HasValue)
            {
                query = query.Where(v => v.ViewingId != excludeViewingId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
