using Skvia.Attendance.Api.Endpoints.Employees.Requests;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignWeeklySchedule;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class AssignWeeklySchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/{employeeId:guid}/schedules/weekly", Handle)
            .WithName(nameof(AssignWeeklySchedule))
            .WithSummary("Asignar horario semanal a un empleado")
            .WithDescription("Limpia el horario existente en el rango de fechas para el empleado y registra el nuevo horario asignado.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Handle(
        Guid employeeId,
        AssignWeeklyScheduleRequest request,
        ICommandHandler<AssignWeeklyScheduleCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new AssignWeeklyScheduleCommand(
            employeeId,
            request.StartDate,
            request.EndDate,
            request.Days);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
