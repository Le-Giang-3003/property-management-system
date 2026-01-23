namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class MaintenanceCommentDto
    {
        public int CommentId { get; set; }
        public int RequestId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Comment { get; set; }
        public bool IsInternal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
