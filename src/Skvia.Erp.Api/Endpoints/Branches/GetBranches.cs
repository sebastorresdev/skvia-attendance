using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Branches.DTOs;
using Skvia.Erp.Application.Features.Branches.Queries.GetBranches;

namespace Skvia.Erp.Api.Endpoints.Branches;

public sealed class GetBranches : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetBranches))
            .WithSummary("Obtener sucursales")
            .WithDescription("Obtiene el listado completo de sucursales/sedes registradas en el sistema.")
            .Produces<List<BranchResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        IQueryHandler<GetBranchesQuery, ErrorOr<List<BranchResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetBranchesQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}



