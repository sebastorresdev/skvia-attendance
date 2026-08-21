using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Kiosks;

namespace Skvia.Erp.Api.Endpoints.KioskDevices;

public sealed class VerifyDeviceToken : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/verify", Handle)
            .WithName(nameof(VerifyDeviceToken))
            .WithSummary("Verificar la validez del token de un dispositivo Kiosko")
            .Produces<VerifyDeviceTokenResponse>(StatusCodes.Status200OK)
            .AllowAnonymous();

    private static async Task<IResult> Handle(
        [FromBody] VerifyDeviceTokenRequest request,
        IApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Token))
        {
            return TypedResults.Ok(new VerifyDeviceTokenResponse(false, null, null, null, null));
        }

        var device = await dbContext.KioskDevices
            .AsNoTracking()
            .Include(d => d.Workplace)
            .FirstOrDefaultAsync(d => d.Token == request.Token, cancellationToken);

        if (device is null)
        {
            return TypedResults.Ok(new VerifyDeviceTokenResponse(false, null, null, null, null));
        }

        if (device.Status == KioskDeviceStatus.Revoked)
        {
            return TypedResults.Ok(new VerifyDeviceTokenResponse(
                IsValid: false,
                Name: device.Name,
                WorkplaceId: device.WorkplaceId,
                WorkplaceName: device.Workplace?.Name,
                Status: (int)KioskDeviceStatus.Revoked));
        }

        if (device.Status != KioskDeviceStatus.Linked)
        {
            return TypedResults.Ok(new VerifyDeviceTokenResponse(
                IsValid: false,
                Name: device.Name,
                WorkplaceId: device.WorkplaceId,
                WorkplaceName: device.Workplace?.Name,
                Status: (int)device.Status));
        }

        return TypedResults.Ok(new VerifyDeviceTokenResponse(
            IsValid: true,
            Name: device.Name,
            WorkplaceId: device.WorkplaceId,
            WorkplaceName: device.Workplace?.Name,
            Status: (int)KioskDeviceStatus.Linked));
    }
}

public record VerifyDeviceTokenRequest(string Token);

public record VerifyDeviceTokenResponse(
    bool IsValid,
    string? Name,
    Guid? WorkplaceId,
    string? WorkplaceName,
    int? Status);

