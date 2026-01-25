using Microsoft.EntityFrameworkCore;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL.Repositories.Implementation
{
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Document>> GetDocumentsByEntityAsync(string entityType, int entityId)
        {
            return await _context.Documents
                .Include(d => d.UploadedByUser)
                .Where(d => d.EntityType == entityType && d.EntityId == entityId && !d.IsDeleted)
                .OrderByDescending(d => d.UploadedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetDocumentsByUserAsync(int userId)
        {
            return await _context.Documents
                .Include(d => d.UploadedByUser)
                .Where(d => d.UploadedBy == userId && !d.IsDeleted)
                .OrderByDescending(d => d.UploadedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Document>> GetDocumentsByTypeAsync(string documentType)
        {
            return await _context.Documents
                .Include(d => d.UploadedByUser)
                .Where(d => d.DocumentType == documentType && !d.IsDeleted)
                .OrderByDescending(d => d.UploadedAt)
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
        {
            return await _context.Documents
                .Include(d => d.UploadedByUser)
                .Include(d => d.DeletedByUser)
                .FirstOrDefaultAsync(d => d.DocumentId == documentId);
        }

        public async Task<bool> SoftDeleteAsync(int documentId, int deletedBy)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return false;

            document.IsDeleted = true;
            document.DeletedAt = DateTime.UtcNow;
            document.DeletedBy = deletedBy;

            _context.Documents.Update(document);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RestoreDocumentAsync(int documentId)
        {
            var document = await _context.Documents.FindAsync(documentId);
            if (document == null) return false;

            document.IsDeleted = false;
            document.DeletedAt = null;
            document.DeletedBy = null;

            _context.Documents.Update(document);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Document>> GetDeletedDocumentsAsync()
        {
            return await _context.Documents
                .Include(d => d.UploadedByUser)
                .Include(d => d.DeletedByUser)
                .Where(d => d.IsDeleted)
                .OrderByDescending(d => d.DeletedAt)
                .ToListAsync();
        }
    }
}
