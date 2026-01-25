using PropertyManagementSystem.BLL.DTOs;
using PropertyManagementSystem.DAL.Entities;

namespace PropertyManagementSystem.Web.ViewModels.Dashboard
{
    public class LandlordDashboardViewModel
    {
        // Statistics
        public int TotalProperties { get; set; }
        public int AvailableProperties { get; set; }
        public int RentedProperties { get; set; }
        public int MaintenanceProperties { get; set; }
        public decimal TotalMonthlyRevenue { get; set; }
        public decimal OccupancyRate { get; set; }

        // Maintenance
        public int PendingMaintenanceRequests { get; set; }
        public List<MaintenanceRequestDto> RecentMaintenanceRequests { get; set; } = new();

        // Applications
        public int PendingApplicationsCount { get; set; }
        public List<RentalApplicationDto> RecentApplications { get; set; } = new();

        // Properties
        public List<PropertyDto> RecentProperties { get; set; } = new();

        // Contracts
        public List<LeaseDto> ExpiringContracts { get; set; } = new();
    }
}
