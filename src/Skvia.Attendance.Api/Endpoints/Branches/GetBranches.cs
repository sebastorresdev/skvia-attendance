using Skvia.Attendance.Application.Branches.DTOs;
using Skvia.Attendance.Application.Branches.Queries.GetBranches;
using Skvia.Attendance.Contracts.Branch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class GetBranches : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithSummary("Obtener sedes")
            .WithDescription("Retorna todas las sedes del sistema.");

    private static async Task<IResult> Handle(
        IQueryHandler<GetBranchesQuery, ErrorOr<List<GetBranchResult>>> handler,
        CancellationToken ct)
    {
        var query = new GetBranchesQuery();

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            res => TypedResults.Ok(res.Select(r => r.ToResponse()).ToList()),
            ResultExtensions.ToProblem);
    }
}

public static class GetBranchesExtension
{
    public static BranchResponse ToResponse(this GetBranchResult result)
    {
        return new BranchResponse(result.Id, result.Code, result.Name, result.Address);
    }
}

