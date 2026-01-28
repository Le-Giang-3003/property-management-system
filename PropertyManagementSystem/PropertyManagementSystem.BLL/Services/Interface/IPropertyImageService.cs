using Microsoft.AspNetCore.Http;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IPropertyImageService
    {
        Task<PropertyImage> UploadImageAsync(int propertyId, IFormFile file, bool isThumbnail = false, string? caption = null);
        Task<List<PropertyImage>> UploadMultipleImagesAsync(int propertyId, List<IFormFile> files);
        Task<List<PropertyImage>> GetImagesByPropertyIdAsync(int propertyId);
        Task<bool> DeleteImageAsync(int imageId, int propertyId);
        Task<bool> SetThumbnailAsync(int propertyId, int imageId);
        Task<bool> UpdateCaptionAsync(int imageId, int propertyId, string caption);
    }
}
