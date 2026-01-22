namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class ViewingFilterRequestDto
    {
        public int? PropertyId { get; set; }
        public string? Status { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
