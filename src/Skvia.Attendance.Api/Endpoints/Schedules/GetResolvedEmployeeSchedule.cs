using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;
using Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetResolvedEmployeeSchedule;

namespace Skvia.Attendance.Api.Endpoints.Schedules;

public sealed class GetResolvedEmployeeSchedule : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/employee/{employeeId:guid}/range", Handle)
            .WithName(nameof(GetResolvedEmployeeSchedule))
            .WithSummary("Obtener horario resuelto para un empleado en un rango")
            .WithDescription("Calcula la combinación de horario base + excepciones para un empleado.")
            .Produces<List<ResolvedScheduleDayDto>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        Guid employeeId,
        DateOnly startDate,
        DateOnly endDate,
        IQueryHandler<GetResolvedEmployeeScheduleQuery, ErrorOr<List<ResolvedScheduleDayDto>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetResolvedEmployeeScheduleQuery(employeeId, startDate, endDate);

        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            data => TypedResults.Ok(data),
            errors => errors.ToProblem());
    }
}
