using Fiap.VehicleSales.Main.Domain.Exceptions;

namespace Fiap.VehicleSales.Main.Domain.Entities;

public sealed class PaymentNotification
{
    public Guid Id { get; private set; }
    public string PaymentCode { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty;
    public DateTime ProcessedAt { get; private set; }

    private PaymentNotification() { }

    public PaymentNotification(string paymentCode, string status)
    {
        if (string.IsNullOrWhiteSpace(paymentCode)) throw new DomainException("Código de pagamento é obrigatório.");
        if (status is not ("Completed" or "Canceled")) throw new DomainException("Status deve ser Completed ou Canceled.");

        Id = Guid.NewGuid();
        PaymentCode = paymentCode.Trim();
        Status = status;
        ProcessedAt = DateTime.UtcNow;
    }
}
