using System.Security.Cryptography;
using System.Text;
using Fiap.VehicleSales.Main.Application.DTOs;
using Fiap.VehicleSales.Main.Application.Exceptions;
using Fiap.VehicleSales.Main.Application.UseCases;
using Fiap.VehicleSales.Main.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fiap.VehicleSales.Main.Api.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromServices] ProcessPaymentWebhookUseCase useCase,
        [FromServices] IConfiguration configuration,
        [FromHeader(Name = "X-Webhook-Secret")] string? provided,
        [FromBody] PaymentWebhookRequest request)
    {
        var expected = configuration["Payments:WebhookSecret"] ?? string.Empty;
        if (!SecretsMatch(provided ?? string.Empty, expected)) return Unauthorized(new { message = "Webhook não autorizado." });

        try
        {
            var processed = await useCase.ExecuteAsync(request);
            return Ok(new { processed });
        }
        catch (AppException exception) { return Conflict(new { message = exception.Message }); }
        catch (DomainException exception) { return BadRequest(new { message = exception.Message }); }
        catch (HttpRequestException exception) { return StatusCode(503, new { message = exception.Message }); }
    }

    private static bool SecretsMatch(string provided, string expected)
    {
        if (string.IsNullOrEmpty(provided) || provided.Length != expected.Length) return false;
        return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(provided), Encoding.UTF8.GetBytes(expected));
    }
}
