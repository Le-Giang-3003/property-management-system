using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    /// <summary>
    /// service for managing property images
    /// </summary>
    /// <seealso cref="PropertyManagementSystem.BLL.Services.Interface.IPropertyImageService" />
    public class PropertyImageService : IPropertyImageService
    {
        /// <summary>
        /// The unit of work
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// The env
        /// </summary>
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyImageService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="env">The env.</param>
        public PropertyImageService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        /// <summary>
        /// Uploads the image asynchronous.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <param name="file">The file.</param>
        /// <param name="isThumbnail">if set to <c>true</c> [is thumbnail].</param>
        /// <param name="caption">The caption.</param>
        /// <returns></returns>
        public async Task<PropertyImage> UploadImageAsync(int propertyId, IFormFile file, bool isThumbnail = false, string? caption = null)
        {
            // Validate file
            ValidateImageFile(file);

            // Create directory
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads", "properties", propertyId.ToString());
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // Generate filename
            var extension = Path.GetExtension(file.FileName).ToLower();
            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Get next display order
            var displayOrder = await _unitOfWork.PropertyImages.GetNextDisplayOrderAsync(propertyId);

            // Check if first image
            var existingImages = await _unitOfWork.PropertyImages.GetByPropertyIdAsync(propertyId);
            if (!existingImages.Any())
            {
                isThumbnail = true; // First image is always thumbnail
            }

            // If set as thumbnail, remove flag from others
            if (isThumbnail)
            {
                await _unitOfWork.PropertyImages.SetThumbnailAsync(propertyId, -1); // Clear all
            }

            // Create PropertyImage entity
            var propertyImage = new PropertyImage
            {
                PropertyId = propertyId,
                ImageUrl = $"/uploads/properties/{propertyId}/{fileName}",
                ImagePath = filePath,
                IsThumbnail = isThumbnail,
                DisplayOrder = displayOrder,
                Caption = caption ?? string.Empty,
                UploadedAt = DateTime.UtcNow
            };

            await _unitOfWork.PropertyImages.AddAsync(propertyImage);
            await _unitOfWork.SaveChangesAsync();

            return propertyImage;
        }

        /// <summary>
        /// Uploads the multiple images asynchronous.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <param name="files">The files.</param>
        /// <returns></returns>
        public async Task<List<PropertyImage>> UploadMultipleImagesAsync(int propertyId, List<IFormFile> files)
        {
            var uploadedImages = new List<PropertyImage>();

            foreach (var file in files)
            {
                try
                {
                    var image = await UploadImageAsync(propertyId, file);
                    uploadedImages.Add(image);
                }
                catch
                {
                    // Continue with other files
                }
            }

            return uploadedImages;
        }

        /// <summary>
        /// Gets the images by property identifier asynchronous.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <returns></returns>
        public async Task<List<PropertyImage>> GetImagesByPropertyIdAsync(int propertyId)
        {
            return await _unitOfWork.PropertyImages.GetByPropertyIdAsync(propertyId);
        }

        /// <summary>
        /// Deletes the image asynchronous.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <param name="propertyId">The property identifier.</param>
        /// <returns></returns>
        public async Task<bool> DeleteImageAsync(int imageId, int propertyId)
        {
            var image = await _unitOfWork.PropertyImages.GetByIdAsync(imageId);
            if (image == null || image.PropertyId != propertyId)
                return false;

            // Delete physical file
            if (File.Exists(image.ImagePath))
            {
                File.Delete(image.ImagePath);
            }

            // Delete from database
            return await _unitOfWork.PropertyImages.DeleteAsync(imageId);
        }

        /// <summary>
        /// Sets the thumbnail asynchronous.
        /// </summary>
        /// <param name="propertyId">The property identifier.</param>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        public async Task<bool> SetThumbnailAsync(int propertyId, int imageId)
        {
            return await _unitOfWork.PropertyImages.SetThumbnailAsync(propertyId, imageId);
        }

        /// <summary>
        /// Validates the image file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <exception cref="ArgumentException">
        /// File không hợp lệ
        /// or
        /// Chỉ chấp nhận file ảnh (jpg, jpeg, png, webp, gif)
        /// or
        /// File không được vượt quá 5MB
        /// </exception>
        private void ValidateImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File không hợp lệ");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Chỉ chấp nhận file ảnh (jpg, jpeg, png, webp, gif)");

            // Max 5MB
            if (file.Length > 5 * 1024 * 1024)
                throw new ArgumentException("File không được vượt quá 5MB");
        }
    }
}
