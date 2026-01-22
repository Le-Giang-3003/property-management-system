using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.DAL.Repositories.Interface
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
        Task<IEnumerable<Document>> GetDocumentsByEntityAsync(string entityType, int entityId);
        Task<IEnumerable<Document>> GetDocumentsByUserAsync(int userId);
        Task<IEnumerable<Document>> GetDocumentsByTypeAsync(string documentType);
        Task<Document?> GetDocumentByIdAsync(int documentId);
        Task<bool> SoftDeleteAsync(int documentId, int deletedBy);
        Task<bool> RestoreDocumentAsync(int documentId);
        Task<IEnumerable<Document>> GetDeletedDocumentsAsync();
    }
}
