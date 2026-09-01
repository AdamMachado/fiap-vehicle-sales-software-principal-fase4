using Fiap.VehicleSales.Main.Application.DTOs;
using Fiap.VehicleSales.Main.Application.Exceptions;
using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Application.Mappers;

namespace Fiap.VehicleSales.Main.Application.UseCases;

public sealed class UpdateVehicleUseCase
{
    private readonly IVehicleRepository _repository;
    private readonly ISalesServiceClient _salesService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVehicleUseCase(IVehicleRepository repository, ISalesServiceClient salesService, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _salesService = salesService;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleResponse> ExecuteAsync(Guid id, VehicleRequest request)
    {
        var vehicle = await _repository.GetByIdAsync(id) ?? throw new AppException("Veículo não encontrado.");
        vehicle.Update(request.Brand, request.Model, request.Year, request.Color, request.Price);
        await _salesService.SyncVehicleAsync(vehicle);
        await _repository.UpdateAsync(vehicle);
        await _unitOfWork.CommitAsync();
        return VehicleMapper.ToResponse(vehicle);
    }
}
