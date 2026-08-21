using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.KioskDevices.Commands.RevokeDevice;

namespace Skvia.Erp.Api.Endpoints.KioskDevices;

public sealed class RevokeDevice : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/{id:guid}/revoke", Handle)
            .WithName(nameof(RevokeDevice))
            .WithSummary("Revocar un dispositivo Kiosko")
            .WithDescription("Revoca el acceso de un dispositivo kiosko.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

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



