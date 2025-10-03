using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarParkManagement.Persistence;

public static class ServiceRegistration
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<CarParkManagementDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("CarParkDb")));

        return services;
    }
}
