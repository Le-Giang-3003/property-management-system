using PropertyManagementSystem.BLL.DTOs.Payments;

namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class LandlordPaymentManagementViewModel
    {
        // Summary Statistics
        public decimal TotalCollected { get; set; }
        public decimal PendingPayments { get; set; }
        public decimal Overdue { get; set; }
        public decimal ThisMonth { get; set; }
        public decimal TotalCollectedChangePercent { get; set; }
        public decimal ThisMonthChangePercent { get; set; }

        // Monthly Revenue Data (for chart)
        public List<MonthlyRevenueData> MonthlyRevenue { get; set; } = new();

        // Payment List
        public List<LandlordPaymentDto> Payments { get; set; } = new();
    }

    public class MonthlyRevenueData
    {
        public string Month { get; set; } = null!;
        public decimal Amount { get; set; }
    }
}
