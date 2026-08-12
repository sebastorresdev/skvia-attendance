using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Kiosks;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

public sealed class ReactivateDevice : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/{id:guid}/reactivate", Handle)
            .WithName(nameof(ReactivateDevice))
            .WithSummary("Reactivar un dispositivo Kiosko inhabilitado")
            .Produces(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        IApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var device = await dbContext.KioskDevices
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);

        if (device is null)
        {
            return TypedResults.Problem(
                title: "KioskDevice.NotFound",
                detail: "Dispositivo kiosko no encontrado.",
                statusCode: StatusCodes.Status404NotFound);
        }

        if (device.Status != KioskDeviceStatus.Revoked)
        {
            return TypedResults.Problem(
                title: "KioskDevice.InvalidState",
                detail: "Solo se pueden reactivar kioskos en estado Inactivo / Deshabilitado.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        device.ReactivateFromDisabled();
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
