using Fiap.VehicleSales.Main.Application.DTOs;
using Fiap.VehicleSales.Main.Application.Exceptions;
using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.Application.UseCases;

public sealed class ProcessPaymentWebhookUseCase
{
    private readonly IPaymentNotificationRepository _repository;
    private readonly ISalesServiceClient _salesService;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessPaymentWebhookUseCase(IPaymentNotificationRepository repository, ISalesServiceClient salesService, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _salesService = salesService;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(PaymentWebhookRequest request)
    {
        var status = NormalizeStatus(request.Status);
        var existing = await _repository.GetByPaymentCodeAsync(request.PaymentCode);

        if (existing is not null)
        {
            if (existing.Status == status) return false;
            throw new AppException("O pagamento já foi processado com outro status.");
        }

        var notification = new PaymentNotification(request.PaymentCode, status);
        await _salesService.ProcessPaymentAsync(notification.PaymentCode, notification.Status);
        await _repository.AddAsync(notification);
        await _unitOfWork.CommitAsync();
        return true;
    }

    private static string NormalizeStatus(string status) => status.Trim().ToLowerInvariant() switch
    {
        "completed" or "paid" or "efetuado" => "Completed",
        "canceled" or "cancelled" or "cancelado" => "Canceled",
        _ => throw new AppException("Status inválido. Use Completed ou Canceled.")
    };
}
