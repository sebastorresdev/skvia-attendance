using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Common.DTOs;
using Skvia.Erp.Application.Features.Users.Queries.GetUserPermissions;

namespace Skvia.Erp.Api.Endpoints.Users;

public sealed class GetUserPermissions : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{userId:guid}/permissions", Handle)
            .WithName(nameof(GetUserPermissions))
            .WithSummary("Obtener permisos de usuario")
            .WithDescription("Obtiene el catálogo completo de permisos marcando cuáles tiene asignados el usuario y su origen.")
            .Produces<List<PermissionGroupResponse>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid userId,
        IQueryHandler<GetUserPermissionsQuery, ErrorOr<List<PermissionGroupResponse>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserPermissionsQuery(userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}



