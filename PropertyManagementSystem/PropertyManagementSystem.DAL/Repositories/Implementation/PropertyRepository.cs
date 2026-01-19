using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly PropertyManagementDbContext _context;
        public PropertyRepository(PropertyManagementDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddPropertyAsync(Property property)
        {
            await _context.Properties.AddAsync(property);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            var property = await _context.Properties.FindAsync(id);
            if (property == null)
            {
                return false;
            }
            property.Status = "Deleted";
            _context.Properties.Update(property);
            var result = await _context.SaveChangesAsync();
            return result > 0;

        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            return await _context.Properties
                .Where(p => p.Status != "Deleted")
                .Include(p => p.Landlord)
                .Include(p => p.PropertyImages)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId)
        {
            return await _context.Properties
                .Where(p => p.LandlordId == landlordId && p.Status != "Deleted")
                .Include(p => p.PropertyImages)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Landlord)
                .Include(p => p.PropertyImages)
                .FirstOrDefaultAsync(p => p.Id == id && p.Status != "Deleted");
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string city, string? propertyType, decimal? minRent, decimal? maxRent)
        {
            var query = _context.Properties
                .Where(p => p.Status == "Available" && p.City == city);

            if (!string.IsNullOrEmpty(propertyType))
            {
                query = query.Where(p => p.PropertyType == propertyType);
            }

            if (minRent.HasValue)
            {
                query = query.Where(p => p.BaseRentPrice >= minRent.Value);
            }

            if (maxRent.HasValue)
            {
                query = query.Where(p => p.BaseRentPrice <= maxRent.Value);
            }

            return await query
                .Include(p => p.Landlord)
                .Include(p => p.PropertyImages)
                .ToListAsync();
        }

        public async Task<bool> UpdatePropertyAsync(Property property)
        {
            var existingProperty = await _context.Properties.FindAsync(property.Id);
            if (existingProperty == null)
            {
                return false;
            }

            _context.Entry(existingProperty).CurrentValues.SetValues(property);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
