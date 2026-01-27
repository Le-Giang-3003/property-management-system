using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task<IEnumerable<Property>> GetAllPropertiesAsync();
        Task<Property?> GetPropertyByIdAsync(int id);
        Task<bool> AddPropertyAsync(Property property);
        Task<bool> UpdatePropertyAsync(Property property);
        Task<bool> DeletePropertyAsync(int id);
        Task<IEnumerable<Property>> GetPropertiesByLandlordIdAsync(int landlordId);
        Task<IEnumerable<Property>> SearchPropertiesAsync(string? city, string? propertyType, decimal? minRent, decimal? maxRent, int? minBedrooms = null, string? status = null);
        Task<Property?> GetPropertyWithDetailsAsync(int propertyId);
        Task<IEnumerable<Property>> GetAvailablePropertiesAsync();
        Task<IEnumerable<Property>> GetByLandlordIdAsync(int landlordId);
        Task<int> GetTotalByLandlordAsync(int landlordId);
        Task<int> GetCountByStatusAsync(int landlordId, string status);
        Task<decimal> GetTotalMonthlyRevenueAsync(int landlordId);
        Task<List<Property>> GetByLandlordWithDetailsAsync(int landlordId, int take = 10);

    }
}
