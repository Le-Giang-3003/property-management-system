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

        public Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId)
        {
            throw new NotImplementedException();
        }

        public Task<Property?> GetPropertyByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Property>> SearchPropertiesAsync(string city, string? propertyType, decimal? minRent, decimal? maxRent)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePropertyAsync(Property property)
        {
            throw new NotImplementedException();
        }
    }
}
