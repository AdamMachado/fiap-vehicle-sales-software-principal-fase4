namespace Fiap.VehicleSales.Main.Application.Exceptions;

public sealed class AppException : Exception
{
    public AppException(string message) : base(message) { }
}
