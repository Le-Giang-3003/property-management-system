using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class AIMarketAnalysis
    {
        [Key]
        public int AnalysisId { get; set; }

        [MaxLength(100)]
        public string City { get; set; }

        [MaxLength(100)]
        public string District { get; set; }

        [MaxLength(50)]
        public string PropertyType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AvgRentPrice { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal PriceChangePercent { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal OccupancyRate { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal DemandIndex { get; set; }

        [MaxLength(2000)]
        public string Trends { get; set; } // JSON

        [MaxLength(1000)]
        public string Insights { get; set; }

        public DateTime AnalysisPeriodStart { get; set; }
        public DateTime AnalysisPeriodEnd { get; set; }
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }

}
