using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class BackupHistory
    {
        [Key]
        public int BackupId { get; set; }

        [Required, MaxLength(200)]
        public string FileName { get; set; }

        [Required, MaxLength(500)]
        public string FilePath { get; set; }

        public long FileSize { get; set; }

        [Required, MaxLength(20)]
        public string BackupType { get; set; } // Full, Incremental, Differential

        [Required, MaxLength(20)]
        public string Status { get; set; } // InProgress, Completed, Failed

        [MaxLength(1000)]
        public string Notes { get; set; }

        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }

        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public User CreatedByUser { get; set; }
    }
}
