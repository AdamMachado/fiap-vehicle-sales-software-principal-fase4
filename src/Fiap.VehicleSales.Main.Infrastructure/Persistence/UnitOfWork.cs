using Fiap.VehicleSales.Main.Application.Interfaces;

namespace Fiap.VehicleSales.Main.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly MainDbContext _context;
    public UnitOfWork(MainDbContext context) => _context = context;
    public async Task CommitAsync() => await _context.SaveChangesAsync();
}
