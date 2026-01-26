using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Auth;
public class ResetPasswordViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter password")]
    [MinLength(8, ErrorMessage = "Password must have at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\w\s]).{8,}$",
        ErrorMessage = "Password must contain: uppercase, lowercase, number and special character")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm password")]
    [Compare("NewPassword", ErrorMessage = "Confirm password does not match")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
