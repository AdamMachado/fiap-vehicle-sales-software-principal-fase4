using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.UnitTests.Fakes;

public sealed class FakeVehicleRepository : IVehicleRepository
{
    public List<Vehicle> Items { get; } = [];
    public Task AddAsync(Vehicle vehicle) { Items.Add(vehicle); return Task.CompletedTask; }
    public Task<Vehicle?> GetByIdAsync(Guid id) => Task.FromResult(Items.FirstOrDefault(item => item.Id == id));
    public Task UpdateAsync(Vehicle vehicle) => Task.CompletedTask;
}

public sealed class FakePaymentRepository : IPaymentNotificationRepository
{
    public List<PaymentNotification> Items { get; } = [];
    public Task<PaymentNotification?> GetByPaymentCodeAsync(string code) => Task.FromResult(Items.FirstOrDefault(item => item.PaymentCode == code));
    public Task AddAsync(PaymentNotification notification) { Items.Add(notification); return Task.CompletedTask; }
}

public sealed class FakeSalesServiceClient : ISalesServiceClient
{
    public List<Guid> SyncedVehicles { get; } = [];
    public List<(string Code, string Status)> Payments { get; } = [];
    public Task SyncVehicleAsync(Vehicle vehicle) { SyncedVehicles.Add(vehicle.Id); return Task.CompletedTask; }
    public Task ProcessPaymentAsync(string code, string status) { Payments.Add((code, status)); return Task.CompletedTask; }
}

public sealed class FakeUnitOfWork : IUnitOfWork
{
    public bool Committed { get; private set; }
    public Task CommitAsync() { Committed = true; return Task.CompletedTask; }
}
