using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.Web.ViewModels.User
{
    /// <summary>
    /// vm for verifying OTP.
    /// </summary>
    public class VerifyOtpViewModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the otp.
        /// </summary>
        /// <value>
        /// The otp.
        /// </value>
        [Required(ErrorMessage = "Mã OTP là bắt buộc")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có 6 số")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Mã OTP phải là 6 chữ số")]
        public string Otp { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the otp hash.
        /// </summary>
        /// <value>
        /// The otp hash.
        /// </value>
        public string OtpHash { get; set; } = string.Empty;
    }
}
