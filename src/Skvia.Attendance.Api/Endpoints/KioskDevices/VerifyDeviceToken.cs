using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

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
            return TypedResults.Ok(new VerifyDeviceTokenResponse(false, null, null, null));
        }

        var device = await dbContext.KioskDevices
            .AsNoTracking()
            .Include(d => d.Workplace)
            .FirstOrDefaultAsync(d => d.Token == request.Token && d.IsActive, cancellationToken);

        if (device is null)
        {
            return TypedResults.Ok(new VerifyDeviceTokenResponse(false, null, null, null));
        }

        return TypedResults.Ok(new VerifyDeviceTokenResponse(
            IsValid: true,
            Name: device.Name,
            WorkplaceId: device.WorkplaceId,
            WorkplaceName: device.Workplace.Name));
    }
}

public record VerifyDeviceTokenRequest(string Token);

public record VerifyDeviceTokenResponse(
    bool IsValid,
    string? Name,
    Guid? WorkplaceId,
    string? WorkplaceName);
