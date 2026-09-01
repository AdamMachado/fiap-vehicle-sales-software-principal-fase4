using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.IntegrationTests;

public sealed class FakeSalesServiceClient : ISalesServiceClient
{
    public List<Guid> Vehicles { get; } = [];
    public List<(string Code, string Status)> Payments { get; } = [];
    public Task SyncVehicleAsync(Vehicle vehicle) { Vehicles.Add(vehicle.Id); return Task.CompletedTask; }
    public Task ProcessPaymentAsync(string code, string status) { Payments.Add((code, status)); return Task.CompletedTask; }
}
