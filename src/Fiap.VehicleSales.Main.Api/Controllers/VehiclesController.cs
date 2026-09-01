using Fiap.VehicleSales.Main.Application.DTOs;
using Fiap.VehicleSales.Main.Application.Exceptions;
using Fiap.VehicleSales.Main.Application.UseCases;
using Fiap.VehicleSales.Main.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.VehicleSales.Main.Api.Controllers;

[ApiController]
[Route("api/vehicles")]
[Authorize(Roles = "admin")]
public sealed class VehiclesController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromServices] CreateVehicleUseCase useCase, [FromBody] VehicleRequest request)
    {
        try
        {
            var response = await useCase.ExecuteAsync(request);
            return Created($"/api/vehicles/{response.Id}", response);
        }
        catch (DomainException exception) { return BadRequest(new { message = exception.Message }); }
        catch (HttpRequestException exception) { return StatusCode(503, new { message = exception.Message }); }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromServices] UpdateVehicleUseCase useCase, [FromBody] VehicleRequest request)
    {
        try
        {
            return Ok(await useCase.ExecuteAsync(id, request));
        }
        catch (AppException exception) { return NotFound(new { message = exception.Message }); }
        catch (DomainException exception) { return BadRequest(new { message = exception.Message }); }
        catch (HttpRequestException exception) { return StatusCode(503, new { message = exception.Message }); }
    }
}
