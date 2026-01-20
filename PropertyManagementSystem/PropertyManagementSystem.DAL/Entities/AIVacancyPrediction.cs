using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class AIVacancyPrediction
    {
        [Key]
        public int PredictionId { get; set; }
        
        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal VacancyRisk { get; set; } // 0-100
        
        public int PredictedDaysVacant { get; set; }
        
        [MaxLength(20)]
        public string RiskLevel { get; set; } // Low, Medium, High
        
        [MaxLength(2000)]
        public string Factors { get; set; } // JSON
        
        [MaxLength(1000)]
        public string Recommendations { get; set; }
        
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation
        public Property Property { get; set; }
    }
}
