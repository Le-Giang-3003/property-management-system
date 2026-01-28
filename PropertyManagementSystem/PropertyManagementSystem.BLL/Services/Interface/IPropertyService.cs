using PropertyManagementSystem.BLL.DTOs.Property;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IPropertyService
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(int id);
        Task<bool> AddPropertyAsync(Property property);
        Task<bool> UpdatePropertyAsync(Property property);
        Task<bool> DeletePropertyAsync(int id);
        Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId);
        Task<IEnumerable<Property>> SearchPropertiesAsync(PropertySearchDto property);
    }
}
