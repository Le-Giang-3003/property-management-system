using PropertyManagementSystem.BLL.DTOs.Payments;
namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class AdminDisputeManagementViewModel
    {
        public List<PaymentDisputeDTO> PendingDisputes { get; set; } = new();
        public List<PaymentDisputeDTO> ResolvedDisputes { get; set; } = new();
    }
}
