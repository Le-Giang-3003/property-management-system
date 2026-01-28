using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class LeaseSignature
    {
        [Key]
        public int SignatureId { get; set; }

        [ForeignKey("Lease")]
        public int LeaseId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required, MaxLength(20)]
        public string SignerRole { get; set; } // Landlord, Tenant

        [Column(TypeName = "nvarchar(max)")]
        public string SignatureData { get; set; } // Base64 or file path

        [MaxLength(50)]
        public string IpAddress { get; set; }

        public DateTime SignedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Lease Lease { get; set; }
        public User User { get; set; }
    }
}
