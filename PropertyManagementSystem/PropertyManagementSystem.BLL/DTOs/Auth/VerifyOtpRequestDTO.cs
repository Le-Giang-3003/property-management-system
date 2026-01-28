using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Auth
{
    public class VerifyOtpRequestDTO
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
        public string OtpCode { get; set; } = null!;
    }
}
