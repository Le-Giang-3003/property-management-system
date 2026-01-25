namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class ResolveDisputeDTO
    {
        public int DisputeId { get; set; }
        public string Resolution { get; set; } = null!; 
        public string Status { get; set; } = "Resolved"; 
    }
}
