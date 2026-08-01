using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.DTOs;
using Skvia.Attendance.Application.Features.Users.Queries.GetUserPermissions;

namespace Skvia.Attendance.Api.Endpoints.Users;

public sealed class GetUserPermissions : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapGet("/{userId:guid}/permissions", Handle)
            .WithName(nameof(GetUserPermissions))
            .WithSummary("Obtener permisos de usuario")
            .WithDescription("Obtiene el catálogo completo de permisos marcando cuáles tiene asignados el usuario y su origen.")
            .Produces<List<PermissionGroupDto>>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid userId,
        IQueryHandler<GetUserPermissionsQuery, ErrorOr<List<PermissionGroupDto>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetUserPermissionsQuery(userId);
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            errors => errors.ToProblem());
    }
}
