using PropertyManagementSystem.BLL.DTOs.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs
{
    public class LandlordDashboardDto
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
    public class PropertyDto
    {
        public int PropertyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal SquareFeet { get; set; }
        public decimal RentAmount { get; set; }
        public string? ThumbnailUrl { get; set; }
    }

    public class RentalApplicationDto
    {
        public int ApplicationId { get; set; }
        public string ApplicantName { get; set; }
        public string PropertyName { get; set; }
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; }
    }

    public class MaintenanceRequestDto
    {
        public int RequestId { get; set; }
        public string Title { get; set; }
        public string PropertyName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
    }

    public class LeaseDto
    {
        public int LeaseId { get; set; }
        public string PropertyName { get; set; }
        public string TenantName { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
