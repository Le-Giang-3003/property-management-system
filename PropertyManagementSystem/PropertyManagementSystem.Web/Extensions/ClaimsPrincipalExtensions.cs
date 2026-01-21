using System.Security.Claims;

namespace PropertyManagementSystem.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out int id) ? id : null;
        }

        public static string? GetUserEmail(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Email)?.Value;

        public static string? GetUserName(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Name)?.Value;
    }
}
