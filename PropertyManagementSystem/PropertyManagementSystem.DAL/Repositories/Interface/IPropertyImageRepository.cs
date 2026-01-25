using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IPropertyImageRepository : IGenericRepository<PropertyImage>
    {
        Task<List<PropertyImage>> GetByPropertyIdAsync(int propertyId);
        Task<PropertyImage?> GetByIdAsync(int imageId);
        Task<bool> DeleteAsync(int imageId);
        Task<bool> SetThumbnailAsync(int propertyId, int imageId);
        Task<int> GetNextDisplayOrderAsync(int propertyId);
    }
}
