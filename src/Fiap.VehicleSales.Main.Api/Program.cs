using Fiap.VehicleSales.Main.Application.UseCases;
using Fiap.VehicleSales.Main.Infrastructure;
using Fiap.VehicleSales.Main.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<CreateVehicleUseCase>();
builder.Services.AddScoped<UpdateVehicleUseCase>();
builder.Services.AddScoped<ProcessPaymentWebhookUseCase>();
builder.Services.AddHealthChecks();

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Keycloak:Authority"];
        var metadataAddress = builder.Configuration["Keycloak:MetadataAddress"];
        if (!string.IsNullOrWhiteSpace(metadataAddress)) options.MetadataAddress = metadataAddress;
        options.Audience = builder.Configuration["Keycloak:Audience"];
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters.ValidateAudience = false;
    });
}

builder.Services.AddAuthorization();
var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    await using var scope = app.Services.CreateAsyncScope();
    await scope.ServiceProvider.GetRequiredService<MainDbContext>().Database.MigrateAsync();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health/live");
app.Run();

public partial class Program { }
