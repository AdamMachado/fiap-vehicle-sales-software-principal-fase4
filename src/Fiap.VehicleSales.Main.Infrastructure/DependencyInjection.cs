using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Infrastructure.Http;
using Fiap.VehicleSales.Main.Infrastructure.Persistence;
using Fiap.VehicleSales.Main.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.VehicleSales.Main.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<MainDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("MainDb")));
        services.Configure<ServiceOptions>(configuration.GetSection("Services"));
        services.AddHttpClient("Keycloak");
        services.AddHttpClient("SalesService", client =>
            client.BaseAddress = new Uri(configuration["Services:SalesServiceUrl"]!));
        services.AddSingleton<ServiceTokenProvider>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IPaymentNotificationRepository, PaymentNotificationRepository>();
        services.AddScoped<ISalesServiceClient, SalesServiceClient>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
