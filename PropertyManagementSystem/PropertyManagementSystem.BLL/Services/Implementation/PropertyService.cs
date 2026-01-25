using PropertyManagementSystem.BLL.DTOs.Property;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _repo;
        public PropertyService(IPropertyRepository repo)
        {
            _repo = repo;
        }
        public async Task<bool> AddPropertyAsync(Property property)
        {
            // Business logic validation
            if (string.IsNullOrWhiteSpace(property.Name))
            {
                throw new ArgumentException("Property title is required");
            }

            // Set default values
            property.CreatedAt = DateTime.UtcNow;
            property.UpdatedAt = DateTime.UtcNow;
            property.Status = "Available";

            return await _repo.AddPropertyAsync(property);
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            var property = await _repo.GetPropertyByIdAsync(id);
            if (property == null)
            {
                throw new KeyNotFoundException($"Property with ID {id} not found");
            }

            return await _repo.DeletePropertyAsync(id);
        }

        public async Task<IEnumerable<Property>> GetAllPropertiesAsync()
        {
            return await _repo.GetAllPropertiesAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId)
        {
            if (landlordId <= 0)
            {
                throw new ArgumentException("Invalid landlord ID");
            }

            return await _repo.GetPropertiesByLandlordIdAsync(landlordId);
        }

        public async Task<Property?> GetPropertyByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Invalid property ID");
            }

            return await _repo.GetPropertyByIdAsync(id);
        }

        public async Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchDto property)
        {

            if (property.MinRent.HasValue && property.MinRent.Value < 0)
            {
                throw new ArgumentException("Minimum rent cannot be negative");
            }

            if (property.MaxRent.HasValue && property.MaxRent.Value < 0)
            {
                throw new ArgumentException("Maximum rent cannot be negative");
            }

            if (property.MinRent.HasValue && property.MaxRent.HasValue && property.MinRent > property.MaxRent)
            {
                throw new ArgumentException("Minimum rent cannot be greater than maximum rent");
            }

            return await _repo.SearchPropertiesAsync(property.City, property.PropertyType, property.MinRent, property.MaxRent);
        }

        public async Task<bool> UpdatePropertyAsync(Property property)
        {
            if (property.PropertyId <= 0)
            {
                throw new ArgumentException("Invalid property ID");
            }

            var existingProperty = await _repo.GetPropertyByIdAsync(property.PropertyId);
            if (existingProperty == null)
            {
                throw new KeyNotFoundException($"Property with ID {property.PropertyId} not found");
            }

            // Business validation
            if (string.IsNullOrWhiteSpace(property.Name))
            {
                throw new ArgumentException("Property title is required");
            }

            property.UpdatedAt = DateTime.UtcNow;

            return await _repo.UpdatePropertyAsync(property);
        }
    }
}
