using System.Net;
using System.Net.Http.Json;
using Fiap.VehicleSales.Main.Application.DTOs;

namespace Fiap.VehicleSales.Main.IntegrationTests;

public sealed class ApiTests
{
    [Fact]
    public async Task Admin_ShouldCreateAndUpdateVehicleAndSynchronize()
    {
        await using var factory = new MainApiFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "admin");

        var create = await client.PostAsJsonAsync("/api/vehicles", Vehicle("Corolla"));
        var created = await create.Content.ReadFromJsonAsync<VehicleResponse>();
        var update = await client.PutAsJsonAsync($"/api/vehicles/{created!.Id}", Vehicle("Corolla XEi"));
        var updated = await update.Content.ReadFromJsonAsync<VehicleResponse>();

        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        Assert.Equal("Corolla XEi", updated!.Model);
        Assert.Equal(2, factory.SalesService.Vehicles.Count);
    }

    [Fact]
    public async Task Buyer_ShouldNotManageCatalog()
    {
        await using var factory = new MainApiFactory();
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Test-Role", "buyer");
        var response = await client.PostAsJsonAsync("/api/vehicles", Vehicle("Corolla"));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Webhook_ShouldRequireSecretAndBeIdempotent()
    {
        await using var factory = new MainApiFactory();
        var client = factory.CreateClient();
        var request = new PaymentWebhookRequest { PaymentCode = "pay-1", Status = "Completed" };

        var unauthorized = await client.PostAsJsonAsync("/api/payments/webhook", request);
        client.DefaultRequestHeaders.Add("X-Webhook-Secret", "test-secret");
        var first = await client.PostAsJsonAsync("/api/payments/webhook", request);
        var repeated = await client.PostAsJsonAsync("/api/payments/webhook", request);

        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, repeated.StatusCode);
        Assert.Single(factory.SalesService.Payments);
    }

    [Fact]
    public async Task Health_ShouldBeAvailable()
    {
        await using var factory = new MainApiFactory();
        Assert.Equal(HttpStatusCode.OK, (await factory.CreateClient().GetAsync("/health/live")).StatusCode);
    }

    private static VehicleRequest Vehicle(string model) => new()
    {
        Brand = "Toyota", Model = model, Year = 2022, Color = "Prata", Price = 120000m
    };
}
