using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

public sealed class ClaimPairingCode : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/claim-code", Handle)
            .WithName(nameof(ClaimPairingCode))
            .WithSummary("Vincular pantalla Kiosko mediante código")
            .WithDescription("Recibe el código ingresado en el kiosko y retorna las credenciales de vinculación si es válido.")
            .Produces<ClaimPairingCodeResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .AllowAnonymous();

    private static async Task<IResult> Handle(
        [FromBody] ClaimPairingCodeRequest request,
        IKioskPairingService pairingService,
        IApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return TypedResults.Problem(
                title: "PairingCode.Required",
                detail: "Por favor ingrese el código de vinculación.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var state = pairingService.ClaimPairingCode(request.Code);
        if (state is null)
        {
            return TypedResults.Problem(
                title: "PairingCode.InvalidOrExpired",
                detail: "El código de vinculación es incorrecto o ha expirado. Genera uno nuevo desde el panel de administración.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        if (state.DeviceId.HasValue)
        {
            var device = await dbContext.KioskDevices
                .FirstOrDefaultAsync(d => d.Id == state.DeviceId.Value, cancellationToken);

            if (device is not null)
            {
                device.MarkAsLinked();
                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        return TypedResults.Ok(new ClaimPairingCodeResponse(
            state.Token,
            state.WorkplaceId,
            state.Name,
            state.WorkplaceName));
    }
}

public record ClaimPairingCodeRequest(string Code);

public record ClaimPairingCodeResponse(
    string Token,
    Guid WorkplaceId,
    string Name,
    string? WorkplaceName);
