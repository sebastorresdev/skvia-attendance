using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Common.DTOs;
using Skvia.Erp.Application.Features.Roles.Queries.GetRolePermissions;

namespace Skvia.Erp.Api.Endpoints.Roles;

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



