using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Roles.DTOs;
using Skvia.Attendance.Application.Features.Roles.Queries.GetRoles;

namespace Skvia.Attendance.Api.Endpoints.Roles;

public sealed class GetRoles : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/", Handle)
            .WithName(nameof(GetRoles))
            .WithSummary("Obtener roles")
            .WithDescription("Obtiene el listado de roles del sistema disponibles para asignación de usuarios.")
            .CacheOutput("CatalogCache")
            .Produces<List<RoleResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest);

    private static async Task<IResult> Handle(
        IQueryHandler<GetRolesQuery, ErrorOr<List<RoleResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetRolesQuery();
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
