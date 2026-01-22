using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class CreateViewingRequestDto
    {
        [Required]
        public int PropertyId { get; set; }

        // Tenant đã login thì không cần, Guest thì bắt buộc
        [MaxLength(100)]
        public string? GuestName { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? GuestEmail { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? GuestPhone { get; set; }

        [Required]
        public DateTime PreferredDate1 { get; set; }

        public DateTime? PreferredDate2 { get; set; }
        public DateTime? PreferredDate3 { get; set; }

        [MaxLength(1000)]
        public string? TenantNotes { get; set; }

        public bool IsVirtualViewing { get; set; }
    }
}
