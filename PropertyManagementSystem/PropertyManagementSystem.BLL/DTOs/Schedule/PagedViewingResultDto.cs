namespace PropertyManagementSystem.BLL.DTOs.Schedule
{
    public class PagedViewingResultDto
    {
        public IEnumerable<PropertyViewingDto> Items { get; set; } = new List<PropertyViewingDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}
