using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Domain.Entities;
using Fiap.VehicleSales.Main.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Fiap.VehicleSales.Main.Infrastructure.Repositories;

public sealed class PaymentNotificationRepository : IPaymentNotificationRepository
{
    private readonly MainDbContext _context;
    public PaymentNotificationRepository(MainDbContext context) => _context = context;
    public async Task<PaymentNotification?> GetByPaymentCodeAsync(string paymentCode) =>
        await _context.PaymentNotifications.FirstOrDefaultAsync(item => item.PaymentCode == paymentCode);
    public async Task AddAsync(PaymentNotification notification) => await _context.PaymentNotifications.AddAsync(notification);
}
