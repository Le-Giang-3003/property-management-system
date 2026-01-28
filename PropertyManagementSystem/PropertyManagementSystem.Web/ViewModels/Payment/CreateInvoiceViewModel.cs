using PropertyManagementSystem.BLL.DTOs.Lease;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Payment
{
    public class CreateInvoiceViewModel
    {
        [Required(ErrorMessage = "Please select a lease")]
        [Display(Name = "Lease")]
        public int LeaseId { get; set; }

        [Required(ErrorMessage = "Please enter the additional service amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive number")]
        [Display(Name = "Additional Services Amount")]
        public decimal AdditionalAmount { get; set; } = 0;

        [Display(Name = "Description for Additional Services")]
        [MaxLength(500)]
        public string? AdditionalDescription { get; set; }

        // For display in form
        public List<LeaseForInvoiceDto> AvailableLeases { get; set; } = new();
    }

    public class LeaseForInvoiceDto
    {
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; } = null!;
        public string PropertyName { get; set; } = null!;
        public string PropertyAddress { get; set; } = null!;
        public string TenantName { get; set; } = null!;
        public string TenantEmail { get; set; } = null!;
        public decimal MonthlyRent { get; set; }
        public int PaymentDueDay { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
