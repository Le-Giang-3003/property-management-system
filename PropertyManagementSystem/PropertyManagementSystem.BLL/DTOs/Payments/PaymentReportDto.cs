namespace PropertyManagementSystem.BLL.DTOs.Payments
{
    public class PaymentReportDto
    {
        public decimal TotalPaid { get; set; }
        public int TotalTransactions { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<PaymentDto> Payments { get; set; } = new();
    }
}
