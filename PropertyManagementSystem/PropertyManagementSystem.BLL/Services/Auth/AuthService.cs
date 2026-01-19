using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using PropertyManagementSystem.BLL.DTOs.Auth;
using PropertyManagementSystem.BLL.Interfaces.Auth;
using PropertyManagementSystem.DAL.Entities;
using PropertyManagementSystem.DAL.Interfaces.Role;
using PropertyManagementSystem.DAL.Interfaces.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PropertyManagementSystem.BLL.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            // Get user by email
            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            if (user.Status != "Active")
            {
                throw new UnauthorizedAccessException("Account is not active");
            }

            // Get user roles
            var roles = await _userRepository.GetUserRolesAsync(user.Id);

            if (roles == null || !roles.Any())
            {
                throw new UnauthorizedAccessException("User has no assigned roles");
            }

            // Generate JWT token
            var token = GenerateJwtToken(user, roles);

            return new LoginResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Roles = roles.Select(r => r.Name).ToList(),
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        #region Private Methods

        private string GenerateJwtToken(User user, List<Role> roles)
        {
            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim("UserId", user.Id.ToString())
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
        private bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
        #endregion

    }
}
