using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Attendances.Queries.GetMonthlySummary;

namespace Skvia.Erp.Api.Endpoints.Attendances;

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



