using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class FavoriteProperty
    {
        [Key]
        public int FavoriteId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
        public Property Property { get; set; }
    }

}
