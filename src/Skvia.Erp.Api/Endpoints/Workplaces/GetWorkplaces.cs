using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Features.Workplaces.Queries.GetWorkplaces;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Api.Endpoints.Workplaces;

public sealed class GetWorkplaces : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetWorkplaces))
            .WithSummary("Obtener lugares de marcación")
            .WithDescription("Obtiene la lista de todos los lugares de marcación (Workplaces).")
            .Produces<IReadOnlyList<WorkplaceDto>>(StatusCodes.Status200OK);

    private static async Task<IResult> Handle(
        IQueryHandler<GetWorkplacesQuery, ErrorOr<IReadOnlyList<WorkplaceDto>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetWorkplacesQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            workplaces => TypedResults.Ok(workplaces),
            errors => errors.ToProblem());
    }
}



