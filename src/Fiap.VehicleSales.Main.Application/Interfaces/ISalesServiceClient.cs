using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.Application.Interfaces;

public interface ISalesServiceClient
{
    Task SyncVehicleAsync(Vehicle vehicle);
    Task ProcessPaymentAsync(string paymentCode, string status);
}
