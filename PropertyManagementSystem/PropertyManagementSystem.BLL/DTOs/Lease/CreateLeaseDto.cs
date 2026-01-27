using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropertyManagementSystem.BLL.DTOs.Lease
{
    public class CreateLeaseDto
    {
        [Required(ErrorMessage = "Please select a rental application")]
        public int ApplicationId { get; set; }

        [Required(ErrorMessage = "Please select lease duration")]
        [Range(1, 120, ErrorMessage = "Lease duration must be 1–120 months")]
        [Display(Name = "Lease duration (months)")]
        public int LeaseDurationMonths { get; set; } = 12;

        [Required(ErrorMessage = "Please enter monthly rent")]
        [Range(0, double.MaxValue, ErrorMessage = "Monthly rent must be positive")]
        [Display(Name = "Monthly rent (VND)")]
        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "Please enter security deposit")]
        [Range(0, double.MaxValue, ErrorMessage = "Security deposit must be positive")]
        [Display(Name = "Security deposit (VND)")]
        public decimal SecurityDeposit { get; set; }

        [Display(Name = "Lease terms")]
        public string Terms { get; set; }

        [Display(Name = "Special conditions")]
        public string SpecialConditions { get; set; }

        [Display(Name = "Auto-renew")]
        public bool AutoRenew { get; set; } = false;
    } 
}
