<<<<<<< HEAD
﻿using Microsoft.Extensions.DependencyInjection;

=======
﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystem.BLL.Identity;
using PropertyManagementSystem.BLL.Services.Implementation;
using PropertyManagementSystem.BLL.Services.Interface;
>>>>>>> 7864dd8da4821481c77672150503091864b776b9
namespace PropertyManagementSystem.BLL
{
    public static class DependencyInjection
    {
<<<<<<< HEAD
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {


=======
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
            // Bind EmailSettings from configuration

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
>>>>>>> 7864dd8da4821481c77672150503091864b776b9
            return services;
        }
    }
}
