namespace Fiap.VehicleSales.Main.Infrastructure.Http;

public sealed class ServiceOptions
{
    public string SalesServiceUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}
