using Skvia.Attendance.Application.Features.Permissions.DTOs;
using Skvia.Attendance.Application.Features.Permissions.Queries.GetPermissions;

namespace Skvia.Attendance.Api.Endpoints.Permissions;

public class GetPermissions : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/", handle)
            .WithSummary("Obtiene la lista de permisos en el sistema.")
            .RequireAuthorization()
            .Produces<List<PermissionGroupDto>>();
    }

    private static Task<IResult> handle(
        IQueryHandler<GetPermissionsQuery, ErrorOr<List<PermissionGroupDto>>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetPermissionsQuery();

        var result = handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
