using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required, MaxLength(2000)]
        public string Message { get; set; }

        [MaxLength(50)]
        public string Type { get; set; } // Payment, Maintenance, Viewing, Lease, System, Message, Application

        [MaxLength(20)]
        public string Priority { get; set; } = "Normal"; // Low, Normal, High, Urgent

        public bool IsRead { get; set; } = false;

        [MaxLength(500)]
        public string ActionUrl { get; set; }

        [MaxLength(100)]
        public string Icon { get; set; }

        [MaxLength(50)]
        public string EntityType { get; set; }

        public int? EntityId { get; set; }

        public DateTime? ReadAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }

        // Navigation
        public User User { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}
