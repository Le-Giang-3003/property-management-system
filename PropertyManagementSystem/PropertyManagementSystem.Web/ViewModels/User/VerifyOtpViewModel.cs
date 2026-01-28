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
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is invalid")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the otp.
        /// </summary>
        /// <value>
        /// The otp.
        /// </value>
        [Required(ErrorMessage = "OTP code is required")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must have 6 digits")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be 6 digits")]
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
