using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.User
{
    public class UpdateProfileDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number")]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(500)]
        public string? Avatar { get; set; }
    }
}
