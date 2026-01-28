namespace PropertyManagementSystem.BLL.DTOs
{
    public class TenantDashboardDto
    {
        // Statistics
        public int ActiveLeasesCount { get; set; }
        public int PendingPaymentsCount { get; set; }
        public int OpenMaintenanceCount { get; set; }
        public int SavedPropertiesCount { get; set; }

        // Upcoming Payments
        public List<UpcomingPaymentDto> UpcomingPayments { get; set; } = new();

        // Active Leases
        public List<TenantLeaseDto> ActiveLeases { get; set; } = new();

        // Recent Activity
        public List<ActivityItemDto> RecentActivities { get; set; } = new();
    }

    public class UpcomingPaymentDto
    {
        public int InvoiceId { get; set; }
        public string PropertyName { get; set; } = "";
        public string Description { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
    }

    public class TenantLeaseDto
    {
        public int LeaseId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = "";
        public string PropertyAddress { get; set; } = "";
        public string? PropertyImageUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public string Status { get; set; } = "";
    }

    public class ActivityItemDto
    {
        public string Type { get; set; } = ""; // Payment, Maintenance, Application, Lease
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; }
    }
}
