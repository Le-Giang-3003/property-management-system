namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class MaintenanceRequestDto
    {
        public int RequestId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; }
        public string PropertyAddress { get; set; }
        public int LandlordId { get; set; }
        public int RequestedBy { get; set; }
        public string TenantName { get; set; }
        public string TenantEmail { get; set; }
        public string TenantPhone { get; set; }
        public int? AssignedTo { get; set; }
        public string TechnicianName { get; set; }
        public string TechnicianEmail { get; set; }
        public string TechnicianPhone { get; set; }
        public string RequestNumber { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string TechnicianStatus { get; set; }
        public string ReasonRejectTechnician { get; set; }
        public string ReasonRejectLandlord { get; set; }
        public string TechnicianNote { get; set; }
        public DateTime RequestDate { get; set; }
        public DateOnly? AssignedDate { get; set; }
        public DateTime? RepairDate { get; set; }
        public TimeOnly? TimeFrom { get; set; }
        public TimeOnly? TimeTo { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public string ResolutionNotes { get; set; }
        public int? Rating { get; set; }
        public string TenantFeedback { get; set; }
        public List<MaintenanceImageDto> Images { get; set; }
        public List<MaintenanceCommentDto> Comments { get; set; }
    }
}
