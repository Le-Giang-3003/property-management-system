using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class MaintenanceComment
    {
        [Key]
        public int CommentId { get; set; }

        [ForeignKey("MaintenanceRequest")]
        public int RequestId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required, MaxLength(2000)]
        public string Comment { get; set; }

        public bool IsInternal { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public MaintenanceRequest MaintenanceRequest { get; set; }
        public User User { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}
