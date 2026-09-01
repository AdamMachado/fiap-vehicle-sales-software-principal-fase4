using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.IntegrationTests;

public sealed class TestVehicleRepository : IVehicleRepository
{
    private readonly List<Vehicle> _items = [];
    public Task AddAsync(Vehicle vehicle) { _items.Add(vehicle); return Task.CompletedTask; }
    public Task<Vehicle?> GetByIdAsync(Guid id) => Task.FromResult(_items.FirstOrDefault(item => item.Id == id));
    public Task UpdateAsync(Vehicle vehicle) => Task.CompletedTask;
}

public sealed class TestPaymentRepository : IPaymentNotificationRepository
{
    private readonly List<PaymentNotification> _items = [];
    public Task<PaymentNotification?> GetByPaymentCodeAsync(string code) => Task.FromResult(_items.FirstOrDefault(item => item.PaymentCode == code));
    public Task AddAsync(PaymentNotification notification) { _items.Add(notification); return Task.CompletedTask; }
}

public sealed class TestUnitOfWork : IUnitOfWork
{
    public Task CommitAsync() => Task.CompletedTask;
}
