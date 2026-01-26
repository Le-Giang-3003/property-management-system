using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Lease
{
    public class TerminateLeaseViewModel
    {
        // LEASE INFORMATION (READ-ONLY)
        public int LeaseId { get; set; }
        public string LeaseNumber { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }

        // TERMINATION INFORMATION (INPUT)
        [Required(ErrorMessage = "Termination date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Termination Date")]
        public DateTime TerminationDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Termination reason is required")]
        [MinLength(10, ErrorMessage = "Reason must have at least 10 characters")]
        [Display(Name = "Termination Reason")]
        public string Reason { get; set; } = string.Empty;
    }

}
