using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class FavoritePropertyRepository : GenericRepository<FavoriteProperty>, IFavoritePropertyRepository
    {
        public FavoritePropertyRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FavoriteProperty>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _context.FavoriteProperties
                .Include(fp => fp.Property) // Include Property để có thông tin chi tiết
                .Where(fp => fp.UserId == userId)
                .ToListAsync();
        }

        // Method này nên đổi tên vì logic không khớp với tên
        public async Task<bool> AddToFavoriteAsync(FavoriteProperty favoriteProperty)
        {
            await _context.FavoriteProperties.AddAsync(favoriteProperty);
            return await _context.SaveChangesAsync() > 0;
        }

        // Kiểm tra xem property đã được favorite chưa
        public async Task<bool> IsPropertyFavoritedAsync(int userId, int propertyId)
        {
            return await _context.FavoriteProperties
                .AnyAsync(fp => fp.UserId == userId && fp.PropertyId == propertyId);
        }

        public async Task<bool> RemoveFromFavoriteAsync(int userId, int propertyId)
        {
            var favoriteProperty = await _context.FavoriteProperties
                .FirstOrDefaultAsync(fp => fp.UserId == userId && fp.PropertyId == propertyId);

            if (favoriteProperty == null)
                return false;

            _context.FavoriteProperties.Remove(favoriteProperty);
            return await _context.SaveChangesAsync() > 0;
        }

        // Hoặc nếu bạn muốn xóa bằng entity
        public async Task<bool> RemoveFromFavoriteAsync(FavoriteProperty favoriteProperty)
        {
            _context.FavoriteProperties.Remove(favoriteProperty);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
