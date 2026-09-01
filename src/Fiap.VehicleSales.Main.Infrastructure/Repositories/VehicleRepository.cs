using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Domain.Entities;
using Fiap.VehicleSales.Main.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fiap.VehicleSales.Main.Infrastructure.Repositories;

public sealed class VehicleRepository : IVehicleRepository
{
    private readonly MainDbContext _context;
    public VehicleRepository(MainDbContext context) => _context = context;
    public async Task AddAsync(Vehicle vehicle) => await _context.Vehicles.AddAsync(vehicle);
    public async Task<Vehicle?> GetByIdAsync(Guid id) => await _context.Vehicles.FirstOrDefaultAsync(vehicle => vehicle.Id == id);
    public Task UpdateAsync(Vehicle vehicle) { _context.Vehicles.Update(vehicle); return Task.CompletedTask; }
}
