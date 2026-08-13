using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.CreateScheduleException;
using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Api.Endpoints.Schedules;

public sealed class CreateScheduleException : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/exceptions", Handle)
            .WithName(nameof(CreateScheduleException))
            .WithSummary("Registrar o actualizar excepción de horario")
            .WithDescription("Registra una excepción puntual para un empleado en un día específico.")
            .Produces<Guid>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromBody] CreateScheduleExceptionRequest request,
        ICommandHandler<CreateScheduleExceptionCommand, ErrorOr<Guid>> handler,
        CancellationToken cancellationToken)
    {
        var command = new CreateScheduleExceptionCommand(
            request.EmployeeId,
            request.Date,
            request.DayType,
            request.CustomScheduleId,
            request.IsDayOff,
            request.StartTime,
            request.EndTime,
            request.Reason);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            id => TypedResults.Ok(id),
            errors => errors.ToProblem());
    }
}

public record CreateScheduleExceptionRequest(
    Guid EmployeeId,
    DateOnly Date,
    ScheduleDayType DayType,
    Guid? CustomScheduleId = null,
    bool IsDayOff = false,
    TimeOnly? StartTime = null,
    TimeOnly? EndTime = null,
    string? Reason = null);
