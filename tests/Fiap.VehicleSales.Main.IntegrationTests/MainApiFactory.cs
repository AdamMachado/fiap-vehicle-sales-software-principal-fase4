using Fiap.VehicleSales.Main.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Fiap.VehicleSales.Main.IntegrationTests;

public sealed class MainApiFactory : WebApplicationFactory<Program>
{
    public FakeSalesServiceClient SalesService { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("Payments:WebhookSecret", "test-secret");
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IVehicleRepository>();
            services.RemoveAll<IPaymentNotificationRepository>();
            services.RemoveAll<IUnitOfWork>();
            services.RemoveAll<ISalesServiceClient>();
            services.AddSingleton<IVehicleRepository, TestVehicleRepository>();
            services.AddSingleton<IPaymentNotificationRepository, TestPaymentRepository>();
            services.AddSingleton<IUnitOfWork, TestUnitOfWork>();
            services.AddSingleton<ISalesServiceClient>(SalesService);
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = TestAuthHandler.SchemeName;
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
            }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });
    }
}
