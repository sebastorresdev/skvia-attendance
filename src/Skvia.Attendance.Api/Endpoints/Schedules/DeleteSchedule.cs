using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Schedules.Commands.DeleteSchedule;

namespace Skvia.Attendance.Api.Endpoints.Schedules;

public sealed class DeleteSchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{id:guid}", Handle)
            .WithName(nameof(DeleteSchedule))
            .WithSummary("Eliminar turno base")
            .WithDescription("Elimina un turno predefinido.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid id,
        ICommandHandler<DeleteScheduleCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteScheduleCommand(id);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
