using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Lease
{
    public class UpdateLeaseViewModel
    {
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; }

        [Required]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public int OriginalStartDay { get; set; }

        [Required]
        [Range(1, 120)]
        [Display(Name = "Lease Duration (months)")]
        public int LeaseDurationMonths { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Payment Due Day")]
        public int PaymentDueDay { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Monthly Rent (VND)")]
        public decimal MonthlyRent { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Display(Name = "Security Deposit (VND)")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Lease Terms")]
        public string? Terms { get; set; }

        [Display(Name = "Special Conditions")]
        public string? SpecialConditions { get; set; }

        [Display(Name = "Auto Renew")]
        public bool AutoRenew { get; set; }

        // Read-only information
        public string? PropertyName { get; set; }
        public string? TenantName { get; set; }
        public string? Status { get; set; }
    }
}
