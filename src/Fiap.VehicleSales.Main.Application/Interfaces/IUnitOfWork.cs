namespace Fiap.VehicleSales.Main.Application.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync();
}
