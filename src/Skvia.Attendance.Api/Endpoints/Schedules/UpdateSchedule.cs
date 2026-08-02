using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Schedules.Commands.UpdateSchedule;

namespace Skvia.Attendance.Api.Endpoints.Schedules;

public sealed class UpdateSchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}", Handle)
            .WithName(nameof(UpdateSchedule))
            .WithSummary("Actualizar turno base")
            .WithDescription("Actualiza los datos de un turno predefinido.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid id,
        [FromBody] UpdateScheduleRequest request,
        ICommandHandler<UpdateScheduleCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateScheduleCommand(id, request.Name, request.DefaultStartTime, request.DefaultEndTime);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record UpdateScheduleRequest(string Name, TimeOnly DefaultStartTime, TimeOnly DefaultEndTime);
