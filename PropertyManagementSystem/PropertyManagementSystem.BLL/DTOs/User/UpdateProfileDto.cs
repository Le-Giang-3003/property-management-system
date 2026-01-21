using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.User
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? Avatar { get; set; }
    }
}
