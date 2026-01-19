using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IPropertyRepository
    {
        public Task<IEnumerable<Property>> GetAllPropertiesAsync();
        public Task<Property?> GetPropertyByIdAsync(int id);
        public Task<bool> AddPropertyAsync(Property property);
        public Task<bool> UpdatePropertyAsync(Property property);
        public Task<bool> DeletePropertyAsync(int id);
        public Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId);
        public Task<IEnumerable<Property>> SearchPropertiesAsync(string city, string? propertyType, decimal? minRent, decimal? maxRent);
    }
}
