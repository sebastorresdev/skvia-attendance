using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.GenerateSchedules;
using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class GenerateSchedules : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{id:guid}/schedules/generate", Handle)
            .WithName(nameof(GenerateSchedules))
            .WithSummary("Generar Horarios desde Patrón")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] GenerateSchedulesRequest request,
        ICommandHandler<GenerateSchedulesCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new GenerateSchedulesCommand(id, request.StartDate, request.EndDate, request.Patterns);
        var result = await handler.HandleAsync(command, cancellationToken);
        
        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record GenerateSchedulesRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    List<SchedulePatternInput>? Patterns = null);
