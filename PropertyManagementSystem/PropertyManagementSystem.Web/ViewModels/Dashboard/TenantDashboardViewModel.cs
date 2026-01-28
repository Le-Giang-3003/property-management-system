using PropertyManagementSystem.BLL.DTOs;

namespace PropertyManagementSystem.Web.ViewModels.Dashboard
{
    public class TenantDashboardViewModel
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
}
