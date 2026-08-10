using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Workplaces;
using Skvia.Attendance.Domain.Kiosks;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

public sealed class AuthorizePairingCode : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/authorize-pin", Handle)
            .WithName(nameof(AuthorizePairingCode))
            .WithSummary("Autorizar un dispositivo Kiosko mediante PIN")
            .Produces<AuthorizeDeviceResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        [FromBody] AuthorizePairingCodeRequest request,
        IKioskPairingService pairingService,
        IApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var cleanCode = request.Code.Replace("-", "").Replace(" ", "").Trim();
        var state = pairingService.GetPairingState(cleanCode);
        if (state is null)
        {
            return TypedResults.Problem(
                title: "PairingCode.InvalidOrExpired",
                detail: "El código PIN de vinculación es inválido o ha expirado.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var workplace = await dbContext.Workplaces.FindAsync(new object[] { request.WorkplaceId }, cancellationToken);
        if (workplace is null)
        {
            return TypedResults.Problem(
                title: "Workplace.NotFound",
                detail: "Lugar de marcación no encontrado.",
                statusCode: StatusCodes.Status404NotFound);
        }

        // Generate a secure random token
        var tokenBytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }
        var token = Convert.ToBase64String(tokenBytes);

        var device = KioskDevice.Create(request.Name, request.WorkplaceId, token);
        dbContext.KioskDevices.Add(device);
        await dbContext.SaveChangesAsync(cancellationToken);

        pairingService.ApprovePairingCode(cleanCode, token, request.WorkplaceId);

        return TypedResults.Ok(new AuthorizeDeviceResponse(token));
    }
}

public record AuthorizePairingCodeRequest(
    string Code,
    string Name,
    Guid WorkplaceId);
