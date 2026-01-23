using Microsoft.Extensions.Configuration;
using PropertyManagementSystem.BLL.DTOs;
using PropertyManagementSystem.BLL.Services.Interface;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.BLL.Services.Implementation
{
    public class DocumentService : IDocumentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _uploadBasePath;

        // Allowed extensions by document type
        private readonly Dictionary<string, string[]> _allowedExtensions = new()
        {
            { "Contract", new[] { ".pdf", ".docx" } },
            { "Invoice", new[] { ".pdf", ".xlsx" } },
            { "Receipt", new[] { ".pdf", ".jpg", ".jpeg", ".png" } },
            { "ID", new[] { ".jpg", ".jpeg", ".png", ".pdf" } },
            { "Application", new[] { ".pdf", ".docx" } },
            { "Image", new[] { ".jpg", ".jpeg", ".png", ".webp" } },
            { "Video", new[] { ".mp4" } },
            { "Spreadsheet", new[] { ".xlsx", ".csv" } },
            { "Presentation", new[] { ".pptx" } },
            { "Archive", new[] { ".zip" } },
            { "Other", new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".jpeg", ".png" } }
        };

        // File size limits (bytes)
        private const long MaxImageSize = 5 * 1024 * 1024; // 5MB
        private const long MaxVideoSize = 50 * 1024 * 1024; // 50MB
        private const long MaxDocumentSize = 10 * 1024 * 1024; // 10MB
        private const long MaxArchiveSize = 20 * 1024 * 1024; // 20MB

        public DocumentService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;

            _uploadBasePath = configuration["FileStorage:UploadPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }

        #region Delete

        public async Task<bool> DeleteDocumentAsync(int documentId, int userId, bool isAdmin)
        {
            var document = await _unitOfWork.Documents.GetDocumentByIdAsync(documentId);
            if (document == null || document.IsDeleted)
                return false;

            // Authorization: Only uploader or admin can delete
            if (document.UploadedBy != userId && !isAdmin)
                throw new UnauthorizedAccessException("You don't have permission to delete this document");

            // Soft delete
            return await _unitOfWork.Documents.SoftDeleteAsync(documentId, userId);
        }

        public async Task<bool> PermanentDeleteAsync(int documentId, int userId, bool isAdmin)
        {
            var document = await _unitOfWork.Documents.GetDocumentByIdAsync(documentId);
            if (document == null)
                return false;

            // Authorization: Only admin can permanently delete
            if (!isAdmin)
                throw new UnauthorizedAccessException("Only admin can permanently delete documents");

            // Delete physical file
            if (File.Exists(document.FilePath))
            {
                try
                {
                    File.Delete(document.FilePath);
                }
                catch (Exception ex)
                {
                    // Log error nhưng vẫn tiếp tục xóa record trong DB
                    Console.WriteLine($"Error deleting physical file: {ex.Message}");
                }
            }

            // Delete from database
            await _unitOfWork.Documents.DeleteByIdAsync(document.DocumentId);
            return true;
        }

        public async Task<bool> RestoreDocumentAsync(int documentId)
        {
            return await _unitOfWork.Documents.RestoreDocumentAsync(documentId);
        }

        #endregion

        #region Download

        public async Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int documentId)
        {
            var document = await _unitOfWork.Documents.GetDocumentByIdAsync(documentId);
            if (document == null || document.IsDeleted)
                return null;

            if (!File.Exists(document.FilePath))
                return null;

            var fileBytes = await File.ReadAllBytesAsync(document.FilePath);
            var contentType = GetContentType(document.FileType);

            return (fileBytes, contentType, document.FileName);
        }

        private string GetContentType(string fileType)
        {
            return fileType.ToLowerInvariant() switch
            {
                "jpg" or "jpeg" => "image/jpeg",
                "png" => "image/png",
                "webp" => "image/webp",
                "mp4" => "video/mp4",
                "pdf" => "application/pdf",
                "docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                "csv" => "text/csv",
                "zip" => "application/zip",
                _ => "application/octet-stream"
            };
        }

        #endregion

        #region Query
        public async Task<IEnumerable<Document>> GetDeletedDocumentsAsync()
        {
            return await _unitOfWork.Documents.GetDeletedDocumentsAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
        {
            return await _unitOfWork.Documents.GetDocumentByIdAsync(documentId);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByEntityAsync(string entityType, int entityId)
        {
            return await _unitOfWork.Documents.GetDocumentsByEntityAsync(entityType, entityId);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByTypeAsync(string documentType)
        {
            return await _unitOfWork.Documents.GetDocumentsByTypeAsync(documentType);
        }

        public async Task<IEnumerable<Document>> GetDocumentsByUserAsync(int userId)
        {
            return await _unitOfWork.Documents.GetDocumentsByUserAsync(userId);
        }

        #endregion

        #region Validation & Helpers

        public string GetFileType(string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "jpg",
                ".png" => "png",
                ".webp" => "webp",
                ".mp4" => "mp4",
                ".pdf" => "pdf",
                ".docx" => "docx",
                ".xlsx" => "xlsx",
                ".pptx" => "pptx",
                ".csv" => "csv",
                ".zip" => "zip",
                _ => "other"
            };
        }

        public bool IsValidFileType(string fileName, string documentType, bool isAdmin)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            // Check if zip (admin only)
            if (extension == ".zip" && !isAdmin)
                return false;

            if (_allowedExtensions.TryGetValue(documentType, out var allowedExts))
            {
                return allowedExts.Contains(extension);
            }

            // Default to "Other" category
            return _allowedExtensions["Other"].Contains(extension);
        }

        public bool ValidateFileSize(long fileSize, string fileType)
        {
            return fileType.ToLowerInvariant() switch
            {
                "jpg" or "jpeg" or "png" or "webp" => fileSize <= MaxImageSize,
                "mp4" => fileSize <= MaxVideoSize,
                "pdf" or "docx" or "xlsx" or "pptx" or "csv" => fileSize <= MaxDocumentSize,
                "zip" => fileSize <= MaxArchiveSize,
                _ => fileSize <= MaxDocumentSize
            };
        }

        #endregion

        #region Upload

        public async Task<Document?> UploadDocumentAsync(
            FileUploadDto fileDto,
            string documentType,
            string entityType,
            int entityId,
            int uploadedBy,
            string? description)
        {
            if (fileDto == null || fileDto.FileStream == null || fileDto.FileSize == 0)
                throw new ArgumentException("File is empty");

            var extension = Path.GetExtension(fileDto.FileName).ToLowerInvariant();
            var fileType = GetFileType(extension);

            // Validate file type
            if (!IsValidFileType(fileDto.FileName, documentType, false))
                throw new ArgumentException($"File type {extension} is not allowed for {documentType}");

            // Validate file size
            if (!ValidateFileSize(fileDto.FileSize, fileType))
                throw new ArgumentException("File size exceeds limit");

            // Generate unique filename
            var uniqueFileName = $"{Guid.NewGuid()}{extension}";

            // Create folder structure: uploads/{entityType}/{entityId}/
            var uploadFolder = Path.Combine(_uploadBasePath, entityType.ToLower(), entityId.ToString());

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            var filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await fileDto.FileStream.CopyToAsync(stream);
            }

            // Create database record
            var document = new Document
            {
                DocumentType = documentType,
                EntityType = entityType,
                EntityId = entityId,
                FileName = fileDto.FileName,
                FileUrl = $"/uploads/{entityType.ToLower()}/{entityId}/{uniqueFileName}",
                FilePath = filePath,
                FileType = fileType.ToUpper(),
                FileSize = fileDto.FileSize,
                Description = description ?? "",
                UploadedBy = uploadedBy,
                UploadedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _unitOfWork.Documents.AddAsync(document);
            await _unitOfWork.SaveChangesAsync();
            return document;
        }

        #endregion

    }
}
