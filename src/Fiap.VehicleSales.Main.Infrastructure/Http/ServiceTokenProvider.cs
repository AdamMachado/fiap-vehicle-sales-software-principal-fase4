using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Fiap.VehicleSales.Main.Infrastructure.Http;

public sealed class ServiceTokenProvider
{
    private readonly IHttpClientFactory _factory;
    private readonly ServiceOptions _options;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _token;
    private DateTime _expiresAt;

    public ServiceTokenProvider(IHttpClientFactory factory, IOptions<ServiceOptions> options)
    {
        _factory = factory;
        _options = options.Value;
    }

    public async Task<string> GetTokenAsync()
    {
        if (_token is not null && _expiresAt > DateTime.UtcNow.AddSeconds(30)) return _token;

        await _lock.WaitAsync();
        try
        {
            if (_token is not null && _expiresAt > DateTime.UtcNow.AddSeconds(30)) return _token;
            using var response = await _factory.CreateClient("Keycloak").PostAsync(_options.TokenUrl, new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = _options.ClientId,
                ["client_secret"] = _options.ClientSecret,
                ["grant_type"] = "client_credentials"
            }));
            response.EnsureSuccessStatusCode();
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            _token = document.RootElement.GetProperty("access_token").GetString()
                ?? throw new InvalidOperationException("Keycloak não retornou access_token.");
            var expiresIn = document.RootElement.TryGetProperty("expires_in", out var value) ? value.GetInt32() : 60;
            _expiresAt = DateTime.UtcNow.AddSeconds(expiresIn);
            return _token;
        }
        finally
        {
            _lock.Release();
        }
    }
}
