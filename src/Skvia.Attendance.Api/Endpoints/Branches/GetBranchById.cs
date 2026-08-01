using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Branches.DTOs;
using Skvia.Attendance.Application.Features.Branches.Queries.GetBranchById;

namespace Skvia.Attendance.Api.Endpoints.Branches;

public sealed class GetBranchById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}", Handle)
            .WithName(nameof(GetBranchById))
            .WithSummary("Obtener sucursal por ID")
            .WithDescription("Obtiene los detalles de una sucursal/sede específica mediante su identificador único.")
            .Produces<BranchDetailResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        IQueryHandler<GetBranchByIdQuery, ErrorOr<BranchDetailResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetBranchByIdQuery(id);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
