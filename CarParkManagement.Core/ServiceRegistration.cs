using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarParkManagement.Core;

public static class ServiceRegistration
{
    public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<ChargesConfiguration>().BindConfiguration("Charges");
        services.AddScoped<ICarParkManagementService, CarParkManagementService>();
        services.AddSingleton<IChargeCalculator, ChargeCalculator>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
