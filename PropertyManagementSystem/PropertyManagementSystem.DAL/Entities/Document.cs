using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        [Required, MaxLength(50)]
        public string DocumentType { get; set; } // Contract, Invoice, Receipt, ID, Application, Other

        [Required, MaxLength(50)]
        public string EntityType { get; set; } // Lease, Invoice, Payment, Property, MaintenanceRequest, RentalApplication

        public int EntityId { get; set; }

        [Required, MaxLength(200)]
        public string FileName { get; set; }

        [Required, MaxLength(500)]
        public string FileUrl { get; set; }

        [Required, MaxLength(500)]
        public string FilePath { get; set; }

        [MaxLength(50)]
        public string FileType { get; set; } // PDF, DOCX, JPG, PNG, XLSX

        public long FileSize { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [ForeignKey("UploadedByUser")]
        public int UploadedBy { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        [ForeignKey("DeletedByUser")]
        public int? DeletedBy { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User UploadedByUser { get; set; }
        public User DeletedByUser { get; set; }
    }
}
