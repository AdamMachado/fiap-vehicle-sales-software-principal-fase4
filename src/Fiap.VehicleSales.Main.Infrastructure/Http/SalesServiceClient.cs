using System.Net.Http.Headers;
using System.Net.Http.Json;
using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.Infrastructure.Http;

public sealed class SalesServiceClient : ISalesServiceClient
{
    private readonly IHttpClientFactory _factory;
    private readonly ServiceTokenProvider _tokens;

    public SalesServiceClient(IHttpClientFactory factory, ServiceTokenProvider tokens)
    {
        _factory = factory;
        _tokens = tokens;
    }

    public Task SyncVehicleAsync(Vehicle vehicle) => SendAsync(
        HttpMethod.Put,
        $"api/internal/vehicles/{vehicle.Id}",
        new { vehicle.Brand, vehicle.Model, vehicle.Year, vehicle.Color, vehicle.Price });

    public Task ProcessPaymentAsync(string paymentCode, string status) => SendAsync(
        HttpMethod.Put,
        $"api/sales/payments/{Uri.EscapeDataString(paymentCode)}",
        new { Status = status });

    private async Task SendAsync(HttpMethod method, string path, object body)
    {
        using var request = new HttpRequestMessage(method, path)
        {
            Content = JsonContent.Create(body)
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", await _tokens.GetTokenAsync());
        using var response = await _factory.CreateClient("SalesService").SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var detail = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Serviço de vendas retornou {(int)response.StatusCode}: {detail}");
        }
    }
}
