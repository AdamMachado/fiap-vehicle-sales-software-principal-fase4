namespace Fiap.VehicleSales.Main.Application.DTOs;

public sealed record VehicleResponse(Guid Id, string Brand, string Model, int Year, string Color, decimal Price);
