using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Lease
{
    public class RenewLeaseViewModel
    {
        // READ-ONLY
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal CurrentMonthlyRent { get; set; }
        public decimal CurrentSecurityDeposit { get; set; }

        // INPUT
        [Required(ErrorMessage = "Please enter the number of extension months")]
        [Range(1, 36, ErrorMessage = "Extension period must be between 1-36 months")]
        [Display(Name = "Extension Months")]
        public int ExtensionMonths { get; set; } = 12;

        [Display(Name = "New Monthly Rent (VND/month)")]
        [Range(0, double.MaxValue, ErrorMessage = "Rent must be greater than 0")]
        public decimal? NewMonthlyRent { get; set; }

        [Display(Name = "New Security Deposit (VND)")]
        [Range(0, double.MaxValue, ErrorMessage = "Security deposit must be greater than 0")]
        public decimal? NewSecurityDeposit { get; set; }

        [Display(Name = "Additional Terms")]
        [StringLength(2000, ErrorMessage = "Terms cannot exceed 2000 characters")]
        public string? AdditionalTerms { get; set; }

        [Display(Name = "Auto Renew")]
        public bool AutoRenew { get; set; }

        // CALCULATED
        public DateTime NewStartDate => EndDate.AddDays(1);
        public DateTime NewEndDate => NewStartDate.AddMonths(ExtensionMonths);
    }
}
