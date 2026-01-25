using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class PropertyImageRepository : GenericRepository<PropertyImage>, IPropertyImageRepository
    {
        public PropertyImageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<PropertyImage>> GetByPropertyIdAsync(int propertyId)
        {
            return await _dbSet
                .Where(i => i.PropertyId == propertyId)
                .OrderBy(i => i.DisplayOrder)
                .ToListAsync();
        }

        public async Task<PropertyImage?> GetByIdAsync(int imageId)
        {
            return await _dbSet.FindAsync(imageId);
        }

        public async Task<bool> DeleteAsync(int imageId)
        {
            var image = await GetByIdAsync(imageId);
            if (image == null) return false;

            _dbSet.Remove(image);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> SetThumbnailAsync(int propertyId, int imageId)
        {
            // Remove thumbnail flag from all images
            var images = await GetByPropertyIdAsync(propertyId);
            foreach (var img in images)
            {
                img.IsThumbnail = (img.ImageId == imageId);
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetNextDisplayOrderAsync(int propertyId)
        {
            var maxOrder = await _dbSet
                .Where(i => i.PropertyId == propertyId)
                .MaxAsync(i => (int?)i.DisplayOrder);

            return (maxOrder ?? -1) + 1;
        }
    }
}
