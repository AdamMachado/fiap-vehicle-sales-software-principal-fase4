using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.Application.Interfaces;

public interface IPaymentNotificationRepository
{
    Task<PaymentNotification?> GetByPaymentCodeAsync(string paymentCode);
    Task AddAsync(PaymentNotification notification);
}
