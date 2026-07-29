using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.DTOs;
using Skvia.Attendance.Application.Features.Users.Queries.GetUserPermissions;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class GetUserPermissions : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet("/{userId:guid}/permissions", handle)
            .WithSummary("Obtiene el catálogo completo de permisos marcando cuáles tiene el usuario y de dónde vienen")
            .Produces<List<PermissionGroupDto>>()
            .Produces<ProblemResponse>(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> handle(
       Guid userId,
       IQueryHandler<GetUserPermissionsQuery, ErrorOr<List<PermissionGroupDto>>> handler,
       CancellationToken cancellationToken)
    {
        var query = new GetUserPermissionsQuery(userId);

        var result = await handler.HandleAsync(query, cancellationToken);

        return result.Match(
            TypedResults.Ok,
            ResultExtensions.ToProblem);
    }
}
