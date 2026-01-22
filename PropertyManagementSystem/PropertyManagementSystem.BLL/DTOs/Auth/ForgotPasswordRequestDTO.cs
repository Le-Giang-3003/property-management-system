using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Auth
{
    public class ForgotPasswordRequestDTO
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP phải có 6 số")]
        public string? OtpCode { get; set; }
        public bool IsOtpSent { get; set; }
    }
}
