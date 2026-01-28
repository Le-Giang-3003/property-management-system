using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class AIPropertyRecommendation
    {
        [Key]
        public int RecommendationId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal MatchScore { get; set; } // 0-100

        [MaxLength(1000)]
        public string MatchReasons { get; set; } // JSON

        public bool IsViewed { get; set; } = false;
        public bool IsDismissed { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; }
        public Property Property { get; set; }
    }
}
