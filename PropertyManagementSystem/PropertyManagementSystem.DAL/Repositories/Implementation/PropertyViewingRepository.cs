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

        public async Task<(IEnumerable<PropertyViewing> Items, int TotalCount)> GetViewingHistoryAsync(
    int? userId,
    string? role,
    string? status,
    int? propertyId,
    DateTime? fromDate,
    DateTime? toDate,
    string? searchTerm,
    int pageNumber,
    int pageSize)
        {
            var query = _dbSet
                .Include(v => v.Property)
                    .ThenInclude(p => p.Landlord)
                .Include(v => v.Tenant)
                .AsQueryable();

            // Chỉ lấy các viewing đã kết thúc (history)
            var historyStatuses = new[] { "Completed", "Cancelled", "Rejected" };
            query = query.Where(v => historyStatuses.Contains(v.Status));

            // Filter theo role
            if (role == "Tenant" && userId.HasValue)
            {
                query = query.Where(v => v.RequestedBy == userId.Value);
            }
            else if (role == "Landlord" && userId.HasValue)
            {
                query = query.Where(v => v.Property.LandlordId == userId.Value);
            }
            // Admin không filter theo userId

            // Filter theo status cụ thể
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(v => v.Status == status);
            }

            // Filter theo property
            if (propertyId.HasValue)
            {
                query = query.Where(v => v.PropertyId == propertyId.Value);
            }

            // Filter theo ngày
            if (fromDate.HasValue)
            {
                query = query.Where(v => v.CreatedAt >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(v => v.CreatedAt <= toDate.Value.AddDays(1));
            }

            // Search theo tên property hoặc tên người yêu cầu
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(v =>
                    v.Property.Name.ToLower().Contains(searchTerm) ||
                    (v.Tenant != null && v.Tenant.FullName.ToLower().Contains(searchTerm)) ||
                    (v.GuestName != null && v.GuestName.ToLower().Contains(searchTerm)));
            }

            // Đếm tổng
            var totalCount = await query.CountAsync();

            // Phân trang
            var items = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<(IEnumerable<PropertyViewing> Items, int TotalCount)> GetAllViewingsAsync(
    string? status,
    int? propertyId,
    DateTime? fromDate,
    DateTime? toDate,
    string? searchTerm,
    int pageNumber,
    int pageSize)
        {
            var query = _dbSet
                .Include(v => v.Property)
                    .ThenInclude(p => p.Landlord)
                .Include(v => v.Tenant)
                .AsQueryable();

            // Filter theo status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(v => v.Status == status);
            }

            // Filter theo property
            if (propertyId.HasValue)
            {
                query = query.Where(v => v.PropertyId == propertyId.Value);
            }

            // Filter theo ngày
            if (fromDate.HasValue)
            {
                query = query.Where(v => v.CreatedAt >= fromDate.Value);
            }
            if (toDate.HasValue)
            {
                query = query.Where(v => v.CreatedAt <= toDate.Value.AddDays(1));
            }

            // Search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(v =>
                    v.Property.Name.ToLower().Contains(searchTerm) ||
                    (v.Tenant != null && v.Tenant.FullName.ToLower().Contains(searchTerm)) ||
                    (v.GuestName != null && v.GuestName.ToLower().Contains(searchTerm)) ||
                    (v.Property.Landlord != null && v.Property.Landlord.FullName.ToLower().Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(v => v.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}
