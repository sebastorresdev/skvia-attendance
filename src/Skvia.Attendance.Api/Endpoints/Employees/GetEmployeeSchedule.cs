using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;
using Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetEmployeeSchedule;

namespace Skvia.Attendance.Api.Endpoints.Employees;

public sealed class GetEmployeeSchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{employeeId:guid}/schedules", Handle)
            .WithName(nameof(GetEmployeeSchedule))
            .WithSummary("Obtener horario del empleado")
            .WithDescription("Obtiene los días de horario asignados a un empleado en un rango de fechas.")
            .Produces<List<EmployeeScheduleResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        Guid employeeId,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        IQueryHandler<GetEmployeeScheduleQuery, ErrorOr<List<EmployeeScheduleResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetEmployeeScheduleQuery(employeeId, startDate, endDate);
        var result = await handler.HandleAsync(query, cancellationToken);
        
        return result.Match(
            schedules => TypedResults.Ok(schedules),
            errors => errors.ToProblem());
    }
}
