using Microsoft.AspNetCore.Mvc;

using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.Commands.SetUserPermissionOverrides;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class SetUserPermissionOverrides : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{userId}/permissions/overrides", handle)
            .WithSummary("Reemplaza los permisos individuales (overrides) del usuario")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemResponse>(StatusCodes.Status409Conflict);
    }

    private static async Task<IResult> handle(
        Guid userId,
        [FromBody] SetUserPermissionOverridesCommand command,
        ICommandHandler<SetUserPermissionOverridesCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
