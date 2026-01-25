namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class RaiseDisputeDTO
    {
        public int InvoiceId { get; set; }
        public string Reason { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}

