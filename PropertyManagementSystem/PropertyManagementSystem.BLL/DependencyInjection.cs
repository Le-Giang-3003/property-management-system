using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystem.BLL.Interfaces.Auth;
using PropertyManagementSystem.BLL.Services.Auth;

namespace PropertyManagementSystem.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
