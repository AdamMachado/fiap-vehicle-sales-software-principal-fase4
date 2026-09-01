using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.Application.Interfaces;

public interface IVehicleRepository
{
    Task AddAsync(Vehicle vehicle);
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task UpdateAsync(Vehicle vehicle);
}
