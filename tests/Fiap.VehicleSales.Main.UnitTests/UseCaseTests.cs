using Fiap.VehicleSales.Main.Application.DTOs;
using Fiap.VehicleSales.Main.Application.Exceptions;
using Fiap.VehicleSales.Main.Application.UseCases;
using Fiap.VehicleSales.Main.Domain.Entities;
using Fiap.VehicleSales.Main.UnitTests.Fakes;

namespace Fiap.VehicleSales.Main.UnitTests;

public sealed class UseCaseTests
{
    [Fact]
    public async Task CreateVehicle_ShouldPersistAndSynchronize()
    {
        var repository = new FakeVehicleRepository();
        var service = new FakeSalesServiceClient();
        var unit = new FakeUnitOfWork();
        var useCase = new CreateVehicleUseCase(repository, service, unit);

        var result = await useCase.ExecuteAsync(VehicleRequest());

        Assert.Single(repository.Items);
        Assert.Contains(result.Id, service.SyncedVehicles);
        Assert.True(unit.Committed);
    }

    [Fact]
    public async Task UpdateVehicle_ShouldPersistAndSynchronize()
    {
        var repository = new FakeVehicleRepository();
        var vehicle = new Vehicle("Toyota", "Corolla", 2022, "Prata", 120000m);
        repository.Items.Add(vehicle);
        var service = new FakeSalesServiceClient();
        var useCase = new UpdateVehicleUseCase(repository, service, new FakeUnitOfWork());

        var result = await useCase.ExecuteAsync(vehicle.Id, VehicleRequest("Corolla XEi"));

        Assert.Equal("Corolla XEi", result.Model);
        Assert.Contains(vehicle.Id, service.SyncedVehicles);
    }

    [Fact]
    public async Task UpdateVehicle_ShouldRejectUnknownId()
    {
        var useCase = new UpdateVehicleUseCase(new FakeVehicleRepository(), new FakeSalesServiceClient(), new FakeUnitOfWork());
        await Assert.ThrowsAsync<AppException>(() => useCase.ExecuteAsync(Guid.NewGuid(), VehicleRequest()));
    }

    [Fact]
    public async Task Webhook_ShouldForwardAndBeIdempotent()
    {
        var repository = new FakePaymentRepository();
        var service = new FakeSalesServiceClient();
        var useCase = new ProcessPaymentWebhookUseCase(repository, service, new FakeUnitOfWork());
        var request = new PaymentWebhookRequest { PaymentCode = "pay-1", Status = "paid" };

        Assert.True(await useCase.ExecuteAsync(request));
        Assert.False(await useCase.ExecuteAsync(request));
        Assert.Single(service.Payments);
        Assert.Equal("Completed", service.Payments[0].Status);
    }

    [Fact]
    public async Task Webhook_ShouldRejectConflictingStatus()
    {
        var repository = new FakePaymentRepository();
        var useCase = new ProcessPaymentWebhookUseCase(repository, new FakeSalesServiceClient(), new FakeUnitOfWork());
        await useCase.ExecuteAsync(new PaymentWebhookRequest { PaymentCode = "pay-1", Status = "Completed" });

        await Assert.ThrowsAsync<AppException>(() => useCase.ExecuteAsync(new PaymentWebhookRequest { PaymentCode = "pay-1", Status = "Canceled" }));
    }

    private static VehicleRequest VehicleRequest(string model = "Corolla") => new()
    {
        Brand = "Toyota", Model = model, Year = 2022, Color = "Prata", Price = 120000m
    };
}
