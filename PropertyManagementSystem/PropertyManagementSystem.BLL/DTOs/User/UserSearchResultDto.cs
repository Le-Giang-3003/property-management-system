namespace PropertyManagementSystem.BLL.DTOs.User
{
    public class UserSearchResultDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
