using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Auth
{
    public class VerifyOtpRequestDTO
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = null!;

        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có đúng 6 chữ số")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Mã OTP phải là 6 chữ số")]
        public string OtpCode { get; set; } = null!;
    }
}
