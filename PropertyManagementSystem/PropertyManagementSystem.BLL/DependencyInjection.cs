using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystem.BLL.Identity;
using PropertyManagementSystem.BLL.Services.Implementation;
using PropertyManagementSystem.BLL.Services.Interface;
namespace PropertyManagementSystem.BLL
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the business logic layer.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Register your business logic layer services here
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IStatelessOtpService, StatelessOtpService>();
            services.AddScoped<IAuthService, AuthService>();
            // Bind EmailSettings from configuration

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
            return services;
        }
    }
}
