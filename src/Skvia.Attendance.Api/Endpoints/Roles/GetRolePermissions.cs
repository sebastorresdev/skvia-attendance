using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Common.DTOs;
using Skvia.Attendance.Application.Features.Roles.Queries.GetRolePermissions;

namespace Skvia.Attendance.Api.Endpoints.Roles;

public class GetRolePermissions : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{id:guid}/permissions", handle)
            .WithName(nameof(GetRolePermissions))
            .WithSummary("Obtener permisos de un rol")
            .WithDescription("Obtiene la lista de permisos asignados a un rol específico.")
            .Produces<List<PermissionGroupResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> handle(
        Guid id,
        IQueryHandler<GetRolePermissionsQuery, ErrorOr<List<PermissionGroupResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetRolePermissionsQuery(id);

        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
