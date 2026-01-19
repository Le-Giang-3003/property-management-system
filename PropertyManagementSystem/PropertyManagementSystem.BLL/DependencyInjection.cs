using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystem.BLL.Services.Implementation;
using PropertyManagementSystem.BLL.Services.Interface;

namespace PropertyManagementSystem.BLL
{
    /// <summary>
    /// DI for Business Logic Layer
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the business logic layer.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <returns></returns>
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            // Register your business logic layer services here
            // e.g., services.AddScoped<IYourService, YourServiceImplementation>();
            services.AddScoped<IPropertyService, PropertyService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }
    }
}
