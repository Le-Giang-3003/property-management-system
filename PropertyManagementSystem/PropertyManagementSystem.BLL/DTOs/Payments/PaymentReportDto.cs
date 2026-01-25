namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class PaymentReportDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? SelectedMethod { get; set; } 
        public decimal TotalPaid { get; set; }
        public int TotalTransactions { get; set; }
        public string? TenantName { get; set; }
        public List<PaymentDto> Payments { get; set; } = new();

    }
}
