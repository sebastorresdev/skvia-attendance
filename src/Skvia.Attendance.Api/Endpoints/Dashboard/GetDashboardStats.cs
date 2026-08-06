using Microsoft.AspNetCore.Mvc;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Dashboard.Queries.GetDashboardStats;
using ErrorOr;

namespace Skvia.Attendance.Api.Endpoints.Dashboard;

public sealed class GetDashboardStats : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/stats", Handle)
            .WithName(nameof(GetDashboardStats))
            .WithSummary("Obtener estadísticas y KPIs del Dashboard")
            .WithDescription("Obtiene los indicadores operativos del día, tendencias semanales y actividad reciente.")
            .Produces<DashboardStatsResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        [FromQuery] Guid? branchId,
        IQueryHandler<GetDashboardStatsQuery, ErrorOr<DashboardStatsResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetDashboardStatsQuery(branchId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            stats => TypedResults.Ok(stats),
            errors => errors.ToProblem());
    }
}
