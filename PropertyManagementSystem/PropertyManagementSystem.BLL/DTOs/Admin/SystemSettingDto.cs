using System.ComponentModel.DataAnnotations;

namespace PropertyManagementSystem.BLL.DTOs.Admin
{
    public class SystemSettingDto
    {
        public int SettingId { get; set; }
        public string SettingKey { get; set; } = string.Empty;
        public string SettingValue { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string DataType { get; set; } = "String";
        public bool IsPublic { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedByName { get; set; }
    }

    public class UpdateSystemSettingDto
    {
        public int SettingId { get; set; }

        [Required]
        public string SettingValue { get; set; } = string.Empty;

        public string? Description { get; set; }
    }

    public class CreateSystemSettingDto
    {
        [Required, MaxLength(100)]
        public string SettingKey { get; set; } = string.Empty;

        [Required]
        public string SettingValue { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Category { get; set; } = "General";

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string DataType { get; set; } = "String";

        public bool IsPublic { get; set; }
    }

    public class AdminDashboardDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalMembers { get; set; }
        public int TotalTechnicians { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalProperties { get; set; }
        public int ActiveLeases { get; set; }
        public int PendingMaintenanceRequests { get; set; }
        public int RecentAuditLogs { get; set; }
        public List<AuditLogDto> LatestAuditLogs { get; set; } = new();
    }
}
