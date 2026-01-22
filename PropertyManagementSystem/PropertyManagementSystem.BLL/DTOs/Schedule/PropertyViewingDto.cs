namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class PropertyViewingDto
    {
        public int ViewingId { get; set; }
        public int PropertyId { get; set; }
        public string PropertyName { get; set; } = string.Empty;
        public string PropertyAddress { get; set; } = string.Empty;
        public string? PropertyThumbnail { get; set; }

        public int? RequestedBy { get; set; }
        public string? TenantName { get; set; }
        public string? TenantEmail { get; set; }
        public string? TenantPhone { get; set; }

        // Guest info (nếu không phải tenant đã đăng ký)
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }

        public string LandlordName { get; set; } = string.Empty;
        public string LandlordEmail { get; set; } = string.Empty;
        public string LandlordPhone { get; set; } = string.Empty;

        public DateTime RequestedDate { get; set; }
        public DateTime? PreferredDate1 { get; set; }
        public DateTime? PreferredDate2 { get; set; }
        public DateTime? PreferredDate3 { get; set; }
        public DateTime? ScheduledDate { get; set; }

        public string? TenantNotes { get; set; }
        public string? LandlordNotes { get; set; }
        public string? MeetingLink { get; set; }

        public string Status { get; set; } = string.Empty;
        public bool IsVirtualViewing { get; set; }

        public int? Rating { get; set; }
        public string? Feedback { get; set; }

        public DateTime CreatedAt { get; set; }

        // Computed
        public string RequesterName => !string.IsNullOrEmpty(TenantName) ? TenantName : GuestName ?? "N/A";
        public string RequesterEmail => !string.IsNullOrEmpty(TenantEmail) ? TenantEmail : GuestEmail ?? "N/A";
        public string RequesterPhone => !string.IsNullOrEmpty(TenantPhone) ? TenantPhone : GuestPhone ?? "N/A";
    }
}
