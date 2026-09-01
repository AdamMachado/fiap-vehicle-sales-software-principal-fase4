using Fiap.VehicleSales.Main.Domain.Entities;
using Fiap.VehicleSales.Main.Domain.Exceptions;

namespace Fiap.VehicleSales.Main.UnitTests;

public sealed class DomainTests
{
    [Fact]
    public void Vehicle_ShouldValidateAndUpdateData()
    {
        var vehicle = new Vehicle(" Toyota ", " Corolla ", 2022, " Prata ", 120000m);
        vehicle.Update("Honda", "Civic", 2021, "Preto", 115000m);

        Assert.Equal("Honda", vehicle.Brand);
        Assert.Equal("Civic", vehicle.Model);
        Assert.NotNull(vehicle.UpdatedAt);
    }

    [Theory]
    [InlineData("", "Model", 2022, "Color", 1)]
    [InlineData("Brand", "", 2022, "Color", 1)]
    [InlineData("Brand", "Model", 1800, "Color", 1)]
    [InlineData("Brand", "Model", 2022, "", 1)]
    [InlineData("Brand", "Model", 2022, "Color", 0)]
    public void Vehicle_ShouldRejectInvalidData(string brand, string model, int year, string color, decimal price)
    {
        Assert.Throws<DomainException>(() => new Vehicle(brand, model, year, color, price));
    }

    [Theory]
    [InlineData("Completed")]
    [InlineData("Canceled")]
    public void PaymentNotification_ShouldAcceptFinalStatus(string status)
    {
        var notification = new PaymentNotification("payment-1", status);
        Assert.Equal(status, notification.Status);
    }
}
