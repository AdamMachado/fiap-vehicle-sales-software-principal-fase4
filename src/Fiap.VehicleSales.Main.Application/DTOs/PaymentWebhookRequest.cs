namespace Fiap.VehicleSales.Main.Application.DTOs;

public sealed class PaymentWebhookRequest
{
    public string PaymentCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
