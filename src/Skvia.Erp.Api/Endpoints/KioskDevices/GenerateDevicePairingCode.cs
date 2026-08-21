using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Kiosks;

namespace Skvia.Erp.Api.Endpoints.KioskDevices;

public sealed class GenerateDevicePairingCode : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/{id:guid}/regenerate-code", Handle)
            .WithName(nameof(GenerateDevicePairingCode))
            .WithSummary("Obtener o regenerar código de vinculación para un dispositivo kiosko")
            .Produces<AuthorizeDeviceResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        [FromQuery] bool force = false,
        IApplicationDbContext dbContext = null!,
        IKioskPairingService pairingService = null!,
        CancellationToken cancellationToken = default)
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

        if (device.Status != KioskDeviceStatus.Pending)
        {
            return TypedResults.Problem(
                title: "KioskDevice.NotPending",
                detail: "Solo se pueden obtener códigos de vinculación para dispositivos en estado Pendiente.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var workplace = await dbContext.Workplaces
            .FirstOrDefaultAsync(w => w.Id == device.WorkplaceId, cancellationToken);

        var workplaceName = workplace?.Name ?? "Sede Desconocida";

        string pairingCode;
        DateTime expiresAt;

        // Reuse existing valid pairing code if force is false
        if (!force && device.IsPairingCodeValid())
        {
            pairingCode = device.PairingCode!;
            expiresAt = device.PairingCodeExpiresAt!.Value;

            pairingService.RegisterPairingCode(
                device.Id,
                device.Name,
                device.WorkplaceId,
                workplaceName,
                device.Token,
                pairingCode,
                expiresAt);
        }
        else
        {
            expiresAt = DateTime.UtcNow.AddMinutes(30);
            pairingCode = pairingService.RegisterPairingCode(
                device.Id,
                device.Name,
                device.WorkplaceId,
                workplaceName,
                device.Token);

            device.SetPairingCode(pairingCode, expiresAt);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return TypedResults.Ok(new AuthorizeDeviceResponse(
            device.Id,
            device.Name,
            device.WorkplaceId,
            workplaceName,
            device.Token,
            pairingCode,
            expiresAt));
    }
}

