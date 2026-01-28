namespace PropertyManagementSystem.BLL.DTOs.Auth
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public UserDto? User { get; set; }
    }
}
