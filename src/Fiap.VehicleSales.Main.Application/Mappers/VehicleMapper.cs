using Fiap.VehicleSales.Main.Application.DTOs;
using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.Application.Mappers;

public static class VehicleMapper
{
    public static VehicleResponse ToResponse(Vehicle vehicle) =>
        new(vehicle.Id, vehicle.Brand, vehicle.Model, vehicle.Year, vehicle.Color, vehicle.Price);
}
