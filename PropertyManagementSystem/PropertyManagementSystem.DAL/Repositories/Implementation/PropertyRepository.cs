using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly AppDbContext _context;
        public PropertyRepository(AppDbContext context)
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
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId)
        {
            return await _context.Properties
                .Where(p => p.LandlordId == landlordId && p.Status != "Deleted")
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            return await _context.Properties
                .Include(p => p.Landlord)
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.PropertyId == id && p.Status != "Deleted");
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(string city, string? propertyType, decimal? minRent, decimal? maxRent)
        {
            var query = _context.Properties
                .Where(p => p.Status == "Available" && p.City == city);

            if (!string.IsNullOrEmpty(propertyType))
            {
                query = query.Where(p => p.PropertyType == propertyType);
            }


            return await query
                .Include(p => p.Landlord)
                .Include(p => p.Images)
                .ToListAsync();
        }

        public async Task<bool> UpdatePropertyAsync(Property property)
        {
            var existingProperty = await _context.Properties.FindAsync(property.PropertyId);
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
