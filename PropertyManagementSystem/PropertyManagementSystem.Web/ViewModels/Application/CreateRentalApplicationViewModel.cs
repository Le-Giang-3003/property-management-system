using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Application
{
    public class CreateRentalApplicationViewModel
    {
        [Required(ErrorMessage = "Please select a property")]
        [Display(Name = "Property")]
        public int PropertyId { get; set; }

        // List of properties for dropdown display
        public IEnumerable<SelectListItem>? AvailableProperties { get; set; }

        public string? PropertyName { get; set; }
        public string? PropertyAddress { get; set; }
        public decimal MonthlyRent { get; set; }

        [Required(ErrorMessage = "Please enter employment status")]
        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; }

        [Display(Name = "Employer/Workplace")]
        [MaxLength(200)]
        public string? Employer { get; set; }

        [Display(Name = "Monthly Income (VND)")]
        [Range(0, double.MaxValue, ErrorMessage = "Income must be a positive number")]
        public decimal? MonthlyIncome { get; set; }

        [Display(Name = "Previous Address")]
        [MaxLength(500)]
        public string? PreviousAddress { get; set; }

        [Display(Name = "Previous Landlord Name")]
        [MaxLength(200)]
        public string? PreviousLandlord { get; set; }

        [RegularExpression(@"^(0[3|5|7|8|9])+([0-9]{8})$",
            ErrorMessage = "Phone number is invalid. Example: 0901234567")]
        [Display(Name = "Previous Landlord Phone")]
        [MaxLength(20)]
        public string? PreviousLandlordPhone { get; set; }

        [Required(ErrorMessage = "Please enter the number of occupants")]
        [Range(1, 20, ErrorMessage = "Number of occupants must be between 1 and 20")]
        [Display(Name = "Number of Occupants")]
        public int NumberOfOccupants { get; set; } = 1;

        [Display(Name = "Do you have pets?")]
        public bool HasPets { get; set; } = false;

        [Display(Name = "Pet Details")]
        [MaxLength(200)]
        public string? PetDetails { get; set; }

        [Required(ErrorMessage = "Please select the desired move-in date")]
        [Display(Name = "Desired Move-in Date")]
        [DataType(DataType.Date)]
        public DateTime DesiredMoveInDate { get; set; }

        [Display(Name = "Additional Notes")]
        [MaxLength(2000)]
        public string? AdditionalNotes { get; set; }
    }
}
