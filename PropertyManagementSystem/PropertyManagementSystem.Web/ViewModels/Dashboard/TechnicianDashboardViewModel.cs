using PropertyManagementSystem.BLL.DTOs.Maintenance;

namespace PropertyManagementSystem.Web.ViewModels.Dashboard
{
    public class TechnicianDashboardViewModel
    {
        public int TotalAssignments { get; set; }
        public int PendingResponse { get; set; }
        public int InProgress { get; set; }
        public int Completed { get; set; }
        public List<MaintenanceRequestDto> RecentRequests { get; set; } = new();
    }
}
