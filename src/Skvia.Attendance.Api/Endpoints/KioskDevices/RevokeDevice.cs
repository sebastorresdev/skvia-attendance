using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.KioskDevices.Commands.RevokeDevice;
using Skvia.Attendance.Domain.Common;
using ErrorOr;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

public sealed class RevokeDevice : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/revoke", Handle)
            .WithName(nameof(RevokeDevice))
            .WithSummary("Revocar un dispositivo Kiosko")
            .WithDescription("Revoca el acceso de un dispositivo kiosko.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<RevokeDeviceCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new RevokeDeviceCommand(id), cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
