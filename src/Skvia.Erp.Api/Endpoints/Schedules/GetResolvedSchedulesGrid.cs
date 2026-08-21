using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.EmployeeSchedules.Queries.GetResolvedSchedulesGrid;

namespace Skvia.Erp.Api.Endpoints.Schedules;

public sealed class GetResolvedSchedulesGrid : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/calendar-grid", Handle)
            .WithName(nameof(GetResolvedSchedulesGrid))
            .WithSummary("Obtener cuadrícula de horarios resueltos")
            .WithDescription("Retorna los horarios resueltos para múltiples empleados en la vista de calendario / rotativo.")
            .Produces<List<EmployeeScheduleGridRowDto>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        DateOnly startDate,
        DateOnly endDate,
        Guid? branchId,
        Guid? departmentId,
        IQueryHandler<GetResolvedSchedulesGridQuery, ErrorOr<List<EmployeeScheduleGridRowDto>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetResolvedSchedulesGridQuery(branchId, departmentId, startDate, endDate);

        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            data => TypedResults.Ok(data),
            errors => errors.ToProblem());
    }
}



