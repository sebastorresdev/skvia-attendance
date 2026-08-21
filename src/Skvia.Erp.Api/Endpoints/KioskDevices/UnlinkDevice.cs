using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Kiosks;

namespace Skvia.Erp.Api.Endpoints.KioskDevices;

public sealed class UnlinkDevice : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/{id:guid}/unlink", Handle)
            .WithName(nameof(UnlinkDevice))
            .WithSummary("Desvincular un dispositivo Kiosko")
            .WithDescription("Desvincula la sesión del equipo físico y cambia el estado a Pendiente de Vinculación.")
            .Produces(StatusCodes.Status204NoContent)
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

        var tokenBytes = new byte[32];
        using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }
        var newToken = Convert.ToBase64String(tokenBytes);

        device.Unlink(newToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}

