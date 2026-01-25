namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class ResolveDisputeViewModel
    {
        public int DisputeId { get; set; }
        public string Resolution { get; set; } = null!;
        public string Status { get; set; } = "Resolved";
        public bool IsRefundRequest { get; set; }
    }
}


