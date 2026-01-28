namespace PropertyManagementSystem.BLL.DTOs.Maintenance
{
    public class MaintenanceImageDto
    {
        public int ImageId { get; set; }
        public int RequestId { get; set; }
        public string ImageUrl { get; set; }
        public string ImagePath { get; set; }
        public string Caption { get; set; }
        public string ImageType { get; set; }
        public int UploadedBy { get; set; }
        public string UploadedByName { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
