using Skvia.Attendance.Application.Branches.DTOs;
using Skvia.Attendance.Application.Branches.Queries.GetBranches;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class GetBranches : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithSummary("Obtener sedes")
            .WithDescription("Retorna todas las sedes del sistema.")
            .Produces<BranchResponse>();

    private static async Task<IResult> Handle(
        IQueryHandler<GetBranchesQuery, ErrorOr<List<BranchResponse>>> handler,
        CancellationToken ct)
    {
        var query = new GetBranchesQuery();

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
