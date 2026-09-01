using System.Net;
using System.Text;
using Fiap.VehicleSales.Main.Domain.Entities;
using Fiap.VehicleSales.Main.Infrastructure.Http;
using Fiap.VehicleSales.Main.Infrastructure.Persistence;
using Fiap.VehicleSales.Main.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fiap.VehicleSales.Main.UnitTests;

public sealed class InfrastructureTests
{
    [Fact]
    public async Task RepositoriesAndUnitOfWork_ShouldPersistEntities()
    {
        var options = new DbContextOptionsBuilder<MainDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var context = new MainDbContext(options);
        var vehicles = new VehicleRepository(context);
        var payments = new PaymentNotificationRepository(context);
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);

        await vehicles.AddAsync(vehicle);
        await payments.AddAsync(new PaymentNotification("pay-1", "Completed"));
        await new UnitOfWork(context).CommitAsync();

        Assert.NotNull(await vehicles.GetByIdAsync(vehicle.Id));
        Assert.NotNull(await payments.GetByPaymentCodeAsync("pay-1"));
        vehicle.Update("Toyota", "Corolla XEi", 2022, "Preto", 118000m);
        await vehicles.UpdateAsync(vehicle);
    }

    [Fact]
    public async Task ServiceTokenProvider_ShouldRequestAndCacheToken()
    {
        var handler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"access_token\":\"token-1\",\"expires_in\":300}", Encoding.UTF8, "application/json")
        });
        var provider = new ServiceTokenProvider(
            new TestHttpClientFactory(new HttpClient(handler)),
            Microsoft.Extensions.Options.Options.Create(ServiceConfig()));

        Assert.Equal("token-1", await provider.GetTokenAsync());
        Assert.Equal("token-1", await provider.GetTokenAsync());
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task SalesServiceClient_ShouldSendCatalogAndPaymentRequests()
    {
        var tokenHandler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{\"access_token\":\"token-1\",\"expires_in\":300}")
        });
        var serviceHandler = new RecordingHandler(_ => new HttpResponseMessage(HttpStatusCode.OK));
        var factory = new NamedHttpClientFactory(new Dictionary<string, HttpClient>
        {
            ["Keycloak"] = new HttpClient(tokenHandler),
            ["SalesService"] = new HttpClient(serviceHandler) { BaseAddress = new Uri("http://sales/") }
        });
        var client = new SalesServiceClient(factory, new ServiceTokenProvider(factory, Microsoft.Extensions.Options.Options.Create(ServiceConfig())));
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);

        await client.SyncVehicleAsync(vehicle);
        await client.ProcessPaymentAsync("pay/1", "Completed");

        Assert.Equal(2, serviceHandler.Requests.Count);
        Assert.All(serviceHandler.Requests, request => Assert.Equal("Bearer", request.AuthorizationScheme));
    }

    private static ServiceOptions ServiceConfig() => new()
    {
        TokenUrl = "http://keycloak/token", ClientId = "main", ClientSecret = "secret", SalesServiceUrl = "http://sales/"
    };

    private sealed class RecordingHandler(Func<HttpRequestMessage, HttpResponseMessage> response) : HttpMessageHandler
    {
        public List<(string Uri, string? AuthorizationScheme)> Requests { get; } = [];
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add((request.RequestUri!.ToString(), request.Headers.Authorization?.Scheme));
            return Task.FromResult(response(request));
        }
    }

    private sealed class TestHttpClientFactory(HttpClient client) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => client;
    }

    private sealed class NamedHttpClientFactory(Dictionary<string, HttpClient> clients) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => clients[name];
    }
}
