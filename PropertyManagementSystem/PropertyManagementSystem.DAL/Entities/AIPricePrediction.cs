using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class AIPricePrediction
    {
        [Key]
        public int PredictionId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SuggestedPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MinPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal MaxPrice { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal ConfidenceLevel { get; set; }

        [MaxLength(2000)]
        public string MarketFactors { get; set; } // JSON

        public DateTime ValidUntil { get; set; }
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Property Property { get; set; }
    }
}
