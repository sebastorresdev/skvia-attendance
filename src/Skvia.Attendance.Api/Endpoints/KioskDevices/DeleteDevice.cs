using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.KioskDevices.Commands.DeleteDevice;

namespace Skvia.Attendance.Api.Endpoints.KioskDevices;

public sealed class DeleteDevice : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{id:guid}", Handle)
            .WithName(nameof(DeleteDevice))
            .WithSummary("Eliminar un dispositivo Kiosko")
            .WithDescription("Elimina permanentemente un dispositivo kiosko.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<DeleteDeviceCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new DeleteDeviceCommand(id), cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
