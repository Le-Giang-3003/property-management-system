using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Auth
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Please enter current password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "Please enter new password")]
        [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
            ErrorMessage = "Password must contain: uppercase, lowercase, number and special character")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Please confirm new password")]
        [Compare("NewPassword", ErrorMessage = "Confirm password does not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = null!;
    }
}
