using Skvia.Attendance.Application.Features.Branches.DTOs;
using Skvia.Attendance.Application.Features.Branches.Queries.GetBranchById;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public class GetBranchById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}", Handle)
                .WithSummary("Obtener sede por ID")
                .WithDescription("Retorna los detalles de una sede específica por su ID.")
                .Produces<BranchDetailResponse>();


    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetBranchByIdQuery, ErrorOr<BranchDetailResponse>> handler,
        CancellationToken ct)
    {
        var query = new GetBranchByIdQuery(id);

        var result = await handler.HandleAsync(query, ct);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
