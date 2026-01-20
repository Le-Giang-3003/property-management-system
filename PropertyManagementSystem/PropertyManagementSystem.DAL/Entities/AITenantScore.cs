using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class AITenantScore
    {
        [Key]
        public int ScoreId { get; set; }

        [ForeignKey("RentalApplication")]
        public int ApplicationId { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal ReliabilityScore { get; set; } // 0-100

        [Column(TypeName = "decimal(5,2)")]
        public decimal IncomeScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal RentalHistoryScore { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal OverallScore { get; set; }

        [MaxLength(20)]
        public string RiskLevel { get; set; } // Low, Medium, High

        [MaxLength(2000)]
        public string Factors { get; set; } // JSON

        [MaxLength(1000)]
        public string Recommendation { get; set; }

        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public RentalApplication RentalApplication { get; set; }
    }
<<<<<<< HEAD
=======

>>>>>>> 7864dd8da4821481c77672150503091864b776b9
}
