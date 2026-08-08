using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Attendances.Queries.ExportAttendancesExcel;


namespace Skvia.Attendance.Api.Endpoints.Attendances;

public sealed class ExportAttendances : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/export", Handle)
            .WithName(nameof(ExportAttendances))
            .WithSummary("Exportar reporte de asistencias a Excel")
            .WithDescription("Genera y descarga un archivo Excel (.xlsx) formateado con los datos de asistencia filtrados.")
            .Produces(StatusCodes.Status200OK, contentType: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        [FromQuery] Guid? branchId,
        [FromQuery] string? employeeSearch,
        [FromQuery] Guid? employeeId,
        [FromQuery] string? statusFilter,
        IQueryHandler<ExportAttendancesExcelQuery, ErrorOr<ExportExcelResult>> handler,
        CancellationToken cancellationToken)
    {
        var query = new ExportAttendancesExcelQuery(startDate, endDate, branchId, employeeSearch, employeeId, statusFilter);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            export => TypedResults.File(export.FileContents, export.ContentType, export.FileName),
            errors => errors.ToProblem());
    }
}
