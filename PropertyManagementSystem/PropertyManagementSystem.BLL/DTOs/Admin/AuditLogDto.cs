namespace PropertyManagementSystem.BLL.DTOs.Admin
{
    public class AuditLogDto
    {
        public int LogId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string Action { get; set; } = string.Empty;
        public string EntityType { get; set; } = string.Empty;
        public int? EntityId { get; set; }
        public string? IpAddress { get; set; }
        public string? Description { get; set; }
        public string? ActiveRole { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AuditLogListDto
    {
        public List<AuditLogDto> Logs { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? SearchTerm { get; set; }
        public string? ActionFilter { get; set; }
        public string? EntityTypeFilter { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}
