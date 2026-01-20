using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class PasswordResetToken
    {
        [Key]
        public int TokenId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required, MaxLength(255)]
        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}
