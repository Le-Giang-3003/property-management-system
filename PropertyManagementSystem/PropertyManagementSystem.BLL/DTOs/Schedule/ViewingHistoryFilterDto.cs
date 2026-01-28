namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class ViewingHistoryFilterDto
    {
        public int? PropertyId { get; set; }
        public string? Status { get; set; } // Completed, Cancelled, Rejected hoặc null = tất cả
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; } // Tìm theo tên property hoặc tên người yêu cầu
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
