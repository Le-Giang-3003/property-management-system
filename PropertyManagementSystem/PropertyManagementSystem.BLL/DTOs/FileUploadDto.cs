namespace PropertyManagementSystem.BLL.DTOs
{
    public class FileUploadDto
    {
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}
