using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PropertyManagementSystem.DAL.Data;
using PropertyManagementSystem.DAL.Interfaces.Role;
using PropertyManagementSystem.DAL.Interfaces.User;
using PropertyManagementSystem.DAL.Repositories.Role;
using PropertyManagementSystem.DAL.Repositories.User;

namespace PropertyManagementSystem.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<PropertyManagementDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("PropertyManagementSystem.DAL")
                ));

            // Register Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            return services;
        }
    }
}
