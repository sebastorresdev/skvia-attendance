using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Dashboard.Queries.GetScheduleAlerts;

namespace Skvia.Attendance.Api.Endpoints.Dashboard;

public sealed class GetScheduleAlerts : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/schedule-alerts", Handle)
            .WithName(nameof(GetScheduleAlerts))
            .WithSummary("Obtener alertas de horarios por vencer")
            .Produces<List<ScheduleAlertDto>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        IQueryHandler<GetScheduleAlertsQuery, ErrorOr<List<ScheduleAlertDto>>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(new GetScheduleAlertsQuery(), cancellationToken);
        
        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
