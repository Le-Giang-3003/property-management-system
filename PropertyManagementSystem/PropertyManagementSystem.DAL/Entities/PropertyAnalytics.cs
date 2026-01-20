using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagementSystem.DAL.Entities
{
    public class PropertyAnalytics
    {
        [Key]
        public int AnalyticsId { get; set; }

        [ForeignKey("Property")]
        public int PropertyId { get; set; }

        public int ViewCount { get; set; } = 0;
        public int ViewingRequestCount { get; set; } = 0;
        public int ApplicationCount { get; set; } = 0;
        public int LeaseCount { get; set; } = 0;

        public int AvgDaysToRent { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AvgMonthlyRevenue { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalRevenue { get; set; } = 0;

        public int MaintenanceRequestCount { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AvgMaintenanceCost { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalMaintenanceCost { get; set; } = 0;

        [Column(TypeName = "decimal(5,2)")]
        public decimal OccupancyRate { get; set; } = 0;

        [Column(TypeName = "decimal(3,2)")]
        public decimal AvgTenantRating { get; set; } = 0;

        public DateTime? LastRentedDate { get; set; }

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Property Property { get; set; }
    }
}
