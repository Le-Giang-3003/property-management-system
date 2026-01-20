namespace PropertyManagementSystem.BLL.DTOs.Auth
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string? Avatar { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
