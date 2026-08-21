using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Schedules.DTOs;
using Skvia.Erp.Application.Features.Schedules.Queries.GetSchedules;

namespace Skvia.Erp.Api.Endpoints.Schedules;

public sealed class GetSchedules : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetSchedules))
            .WithSummary("Obtener turnos base")
            .WithDescription("Obtiene el listado completo de turnos predefinidos.")
            .Produces<List<ScheduleResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        IQueryHandler<GetSchedulesQuery, ErrorOr<List<ScheduleResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetSchedulesQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}



