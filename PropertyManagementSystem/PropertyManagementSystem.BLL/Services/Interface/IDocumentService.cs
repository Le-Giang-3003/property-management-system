using PropertyManagementSystem.BLL.DTOs;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.BLL.Services.Interface
{
    public interface IDocumentService
    {
        // Upload 
        Task<Document?> UploadDocumentAsync(
            FileUploadDto fileDto,
            string documentType,
            string entityType,
            int entityId,
            int uploadedBy,
            string? description);

        // Download
        Task<(byte[] fileBytes, string contentType, string fileName)?> DownloadDocumentAsync(int documentId);

        // Delete
        Task<bool> DeleteDocumentAsync(int documentId, int userId, bool isAdmin);
        Task<bool> PermanentDeleteAsync(int documentId, int userId, bool isAdmin);
        Task<bool> RestoreDocumentAsync(int documentId);

        // Query
        Task<IEnumerable<Document>> GetDocumentsByEntityAsync(string entityType, int entityId);
        Task<IEnumerable<Document>> GetDocumentsByUserAsync(int userId);
        Task<IEnumerable<Document>> GetDocumentsByTypeAsync(string documentType);
        Task<Document?> GetDocumentByIdAsync(int documentId);
        Task<IEnumerable<Document>> GetDeletedDocumentsAsync();

        // Validation
        bool IsValidFileType(string fileName, string documentType, bool isAdmin);
        bool ValidateFileSize(long fileSize, string fileType);
        string GetFileType(string extension);

        Task<IEnumerable<Document>> GetLeaseDocumentsByTenantUserIdAsync(int tenantUserId);
    }
}
