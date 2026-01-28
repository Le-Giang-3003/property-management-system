using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }

        [Required, MaxLength(100)]
        public string Action { get; set; } // Create, Update, Delete, Login, Logout, PasswordChange, etc.

        [Required, MaxLength(100)]
        public string EntityType { get; set; }

        public int? EntityId { get; set; }

        [MaxLength(50)]
        public string IpAddress { get; set; }

        [MaxLength(500)]
        public string UserAgent { get; set; }

        public string OldValues { get; set; } // JSON
        public string NewValues { get; set; } // JSON

        [MaxLength(1000)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string ActiveRole { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
    }
}
