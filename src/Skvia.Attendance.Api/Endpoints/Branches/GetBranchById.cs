using Skvia.Attendance.Application.Branches.DTOs;
using Skvia.Attendance.Application.Branches.Queries.GetBranchById;
using Skvia.Attendance.Contracts.Branch;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class GetBranchById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}", Handle)
                .WithSummary("Obtener sede por ID")
                .WithDescription("Retorna los detalles de una sede específica por su ID.");


    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetBranchByIdQuery, ErrorOr<GetBranchByIdResult>> handler,
        CancellationToken ct)
    {
        var query = new GetBranchByIdQuery(id);

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            branchResult => TypedResults.Ok(branchResult.ToResponse()),
            ResultExtensions.ToProblem);
    }
}

public static class GetBranchByIdExtension
{
    public static BranchDetailResponse ToResponse(this GetBranchByIdResult result)
    {
        return new BranchDetailResponse(result.Id, result.Code, result.Name, result.Address);
    }
}
