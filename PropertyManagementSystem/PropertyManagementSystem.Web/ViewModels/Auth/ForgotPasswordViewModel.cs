using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.Auth
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Email cannot be empty")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; } = string.Empty;

        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must have 6 digits")]
        public string? OtpCode { get; set; }
        public bool IsOtpSent { get; set; }
    }
}
