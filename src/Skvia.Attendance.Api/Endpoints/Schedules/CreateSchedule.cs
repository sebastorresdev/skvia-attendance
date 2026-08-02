using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Schedules.Commands.CreateSchedule;

namespace Skvia.Attendance.Api.Endpoints.Schedules;

public sealed class CreateSchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/", Handle)
            .WithName(nameof(CreateSchedule))
            .WithSummary("Crear turno base")
            .WithDescription("Crea un nuevo turno predefinido.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Handle(
        [FromBody] CreateScheduleRequest request,
        ICommandHandler<CreateScheduleCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateScheduleCommand(request.Name, request.DefaultStartTime, request.DefaultEndTime);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            id => TypedResults.CreatedAtRoute(id, nameof(GetSchedules), new { id }),
            errors => errors.ToProblem());
    }
}

public record CreateScheduleRequest(string Name, TimeOnly DefaultStartTime, TimeOnly DefaultEndTime);
