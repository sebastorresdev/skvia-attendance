using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Roles.DTOs;
using Skvia.Attendance.Application.Features.Roles.Queries.GetRoleById;

namespace Skvia.Attendance.Api.Endpoints.Roles;

public class GetRoleById : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}", handle)
            .WithName(nameof(GetRoleById))
            .WithSummary("Obtener rol por ID")
            .WithDescription("Obtiene los detalles de un rol específico mediante su identificador único.")
            .Produces<RoleResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> handle(
        Guid id,
        IQueryHandler<GetRoleByIdQuery, ErrorOr<RoleResponse>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetRoleByIdQuery(id);

        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
