using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using PropertyManagementSystem.Web.Attributes;

namespace PropertyManagementSystem.Web.ViewModels.Lease
{
    public class CreateLeaseViewModel
    {
        public int ApplicationId { get; set; }

        // Application information (readonly)
        public string? ApplicationNumber { get; set; }
        public string? PropertyName { get; set; }
        public string? PropertyAddress { get; set; }
        public string? TenantName { get; set; }
        public string? TenantEmail { get; set; }
        public string? TenantPhone { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        public int OriginalStartDay { get; set; } // For displaying warning

        [Required(ErrorMessage = "Please select a lease duration")]
        [Display(Name = "Lease Duration")]
        public int LeaseDurationMonths { get; set; } = 12;

        public List<SelectListItem> DurationOptions { get; set; } = new List<SelectListItem>
        {
            new SelectListItem { Value = "3", Text = "3 months" },
            new SelectListItem { Value = "6", Text = "6 months" },
            new SelectListItem { Value = "9", Text = "9 months" },
            new SelectListItem { Value = "12", Text = "12 months (1 year)", Selected = true },
            new SelectListItem { Value = "18", Text = "18 months" },
            new SelectListItem { Value = "24", Text = "24 months (2 years)" },
            new SelectListItem { Value = "36", Text = "36 months (3 years)" }
        };

        [Display(Name = "End Date (estimated)")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Monthly Payment Due Day")]
        public int PaymentDueDay { get; set; }

        [Required(ErrorMessage = "Please enter the rent amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Rent must be a positive number")]
        [MultipleOf1000(ErrorMessage = "Please enter a multiple of 1000")]
        [Display(Name = "Monthly Rent (VND)")]
        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "Please enter the security deposit")]
        [Range(0, double.MaxValue, ErrorMessage = "Security deposit must be a positive number")]
        [MultipleOf1000(ErrorMessage = "Please enter a multiple of 1000")]
        [Display(Name = "Security Deposit (VND)")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Lease Terms")]
        public string? Terms { get; set; }

        [Display(Name = "Special Conditions")]
        public string? SpecialConditions { get; set; }

        [Display(Name = "Auto Renew")]
        public bool AutoRenew { get; set; } = false;
    }

    
}
