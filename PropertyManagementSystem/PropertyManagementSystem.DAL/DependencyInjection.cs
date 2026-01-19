using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Repositories.Implementation;
using PropertyManagementSystem.DAL.Repositories.Interface;

namespace PropertyManagementSystem.DAL
{
    /// <summary>
    /// DI for Data Access Layer
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Adds the data access layer.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns></returns>
        public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
        {
            // Register your data access layer services here
            // e.g., services.AddScoped<IYourRepository, YourRepositoryImplementation>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }
    }
}
