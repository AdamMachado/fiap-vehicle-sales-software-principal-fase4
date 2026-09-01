using Fiap.VehicleSales.Main.Domain.Exceptions;

namespace Fiap.VehicleSales.Main.Domain.Entities;

public sealed class Vehicle
{
    public Guid Id { get; private set; }
    public string Brand { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public string Color { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Vehicle() { }

    public Vehicle(string brand, string model, int year, string color, decimal price)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        SetData(brand, model, year, color, price);
    }

    public void Update(string brand, string model, int year, string color, decimal price)
    {
        SetData(brand, model, year, color, price);
        UpdatedAt = DateTime.UtcNow;
    }

    private void SetData(string brand, string model, int year, string color, decimal price)
    {
        if (string.IsNullOrWhiteSpace(brand)) throw new DomainException("Marca é obrigatória.");
        if (string.IsNullOrWhiteSpace(model)) throw new DomainException("Modelo é obrigatório.");
        if (year < 1900 || year > DateTime.UtcNow.Year + 1) throw new DomainException("Ano inválido.");
        if (string.IsNullOrWhiteSpace(color)) throw new DomainException("Cor é obrigatória.");
        if (price <= 0) throw new DomainException("Preço deve ser maior que zero.");

        Brand = brand.Trim();
        Model = model.Trim();
        Year = year;
        Color = color.Trim();
        Price = price;
    }
}
