using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PropertyManagementSystem.BLL.Services.Interface;
using System.Security.Claims;

namespace PropertyManagementSystem.Web.Middleware
{
    /// <summary>
    /// Middleware to validate that authenticated users still exist in the database.
    /// This prevents access when the database has been reset but authentication cookies still exist.
    /// </summary>
    public class ValidateUserMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidateUserMiddleware> _logger;

        public ValidateUserMiddleware(RequestDelegate next, ILogger<ValidateUserMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            // Skip validation for login, logout, and static files
            if (context.Request.Path.StartsWithSegments("/Auth/Login") ||
                context.Request.Path.StartsWithSegments("/Auth/Logout") ||
                context.Request.Path.StartsWithSegments("/lib") ||
                context.Request.Path.StartsWithSegments("/css") ||
                context.Request.Path.StartsWithSegments("/js") ||
                context.Request.Path.StartsWithSegments("/images") ||
                context.Request.Path.StartsWithSegments("/uploads"))
            {
                await _next(context);
                return;
            }

            // Only validate if user is authenticated
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    try
                    {
                        var user = await userService.GetUserByIdAsync(userId);
                        
                        // If user doesn't exist or is inactive, sign out and redirect to login
                        if (user == null || !user.IsActive)
                        {
                            _logger.LogWarning($"User {userId} not found or inactive. Signing out.");
                            
                            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                            
                            // Clear the authentication cookie
                            context.Response.Cookies.Delete(".AspNetCore.Cookies");
                            
                            context.Response.Redirect("/Auth/Login?message=Session expired. Please login again.");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error validating user {userId}");
                        // On error, continue to next middleware to avoid blocking
                    }
                }
            }

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method to register the middleware
    /// </summary>
    public static class ValidateUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseValidateUser(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ValidateUserMiddleware>();
        }
    }
}
