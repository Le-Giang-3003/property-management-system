namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class MaintenanceStatsDto
    {
        public int TotalRequests { get; set; }
        public int PendingRequests { get; set; }
        public int InProgressRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int CancelledRequests { get; set; }
        public int RejectedRequests { get; set; }
    }
}
