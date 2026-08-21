using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Attendances.Queries.GetAttendances;

namespace Skvia.Erp.Api.Endpoints.Attendances;

public sealed class GetAttendances : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetAttendances))
            .WithSummary("Obtener reporte de asistencias")
            .WithDescription("Obtiene una lista de marcaciones de asistencia aplicando filtros por fecha, sede y empleado.")
            .Produces<List<AttendanceResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        [FromQuery] Guid? branchId,
        [FromQuery] string? employeeSearch,
        [FromQuery] Guid? employeeId,
        [FromQuery] string? statusFilter,
        IQueryHandler<GetAttendancesQuery, ErrorOr<List<AttendanceResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetAttendancesQuery(startDate, endDate, branchId, employeeSearch, employeeId, statusFilter);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            attendances => TypedResults.Ok(attendances),
            errors => errors.ToProblem());
    }
}



