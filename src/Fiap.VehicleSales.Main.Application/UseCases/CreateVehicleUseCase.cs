using Fiap.VehicleSales.Main.Application.DTOs;
using Fiap.VehicleSales.Main.Application.Interfaces;
using Fiap.VehicleSales.Main.Application.Mappers;
using Fiap.VehicleSales.Main.Domain.Entities;

namespace Fiap.VehicleSales.Main.Application.UseCases;

public sealed class CreateVehicleUseCase
{
    private readonly IVehicleRepository _repository;
    private readonly ISalesServiceClient _salesService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateVehicleUseCase(IVehicleRepository repository, ISalesServiceClient salesService, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _salesService = salesService;
        _unitOfWork = unitOfWork;
    }

    public async Task<VehicleResponse> ExecuteAsync(VehicleRequest request)
    {
        var vehicle = new Vehicle(request.Brand, request.Model, request.Year, request.Color, request.Price);
        await _repository.AddAsync(vehicle);
        await _salesService.SyncVehicleAsync(vehicle);
        await _unitOfWork.CommitAsync();
        return VehicleMapper.ToResponse(vehicle);
    }
}
