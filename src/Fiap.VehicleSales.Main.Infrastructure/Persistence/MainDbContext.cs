using Fiap.VehicleSales.Main.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fiap.VehicleSales.Main.Infrastructure.Persistence;

public sealed class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<PaymentNotification> PaymentNotifications => Set<PaymentNotification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Vehicle>(builder =>
        {
            builder.ToTable("Vehicles");
            builder.HasKey(vehicle => vehicle.Id);
            builder.Property(vehicle => vehicle.Brand).HasMaxLength(100).IsRequired();
            builder.Property(vehicle => vehicle.Model).HasMaxLength(100).IsRequired();
            builder.Property(vehicle => vehicle.Color).HasMaxLength(50).IsRequired();
            builder.Property(vehicle => vehicle.Price).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<PaymentNotification>(builder =>
        {
            builder.ToTable("PaymentNotifications");
            builder.HasKey(notification => notification.Id);
            builder.Property(notification => notification.PaymentCode).HasMaxLength(64).IsRequired();
            builder.Property(notification => notification.Status).HasMaxLength(20).IsRequired();
            builder.HasIndex(notification => notification.PaymentCode).IsUnique();
        });
    }
}
