using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Attendances.Queries.GetMonthlySummary;
using ErrorOr;

namespace Skvia.Attendance.Api.Endpoints.Attendances;

public sealed class GetMonthlySummary : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/monthly-summary", Handle)
            .WithName(nameof(GetMonthlySummary))
            .WithSummary("Obtener consolidado mensual de asistencias / Pre-Nómina")
            .WithDescription("Obtiene la matriz mensual acumulada por empleado (días trabajados, faltas, tardanzas y justificaciones).")
            .Produces<MonthlySummaryResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromQuery] int year,
        [FromQuery] int month,
        [FromQuery] Guid? branchId,
        IQueryHandler<GetMonthlySummaryQuery, ErrorOr<MonthlySummaryResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetMonthlySummaryQuery(year, month, branchId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            summary => TypedResults.Ok(summary),
            errors => errors.ToProblem());
    }
}
