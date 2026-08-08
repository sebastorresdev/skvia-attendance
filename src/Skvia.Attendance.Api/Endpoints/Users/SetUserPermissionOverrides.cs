using Microsoft.AspNetCore.Mvc;

using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.Commands.SetUserPermissionOverrides;

namespace Skvia.Attendance.Api.Endpoints.Users;

public sealed class SetUserPermissionOverrides : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{userId:guid}/permissions/overrides", Handle)
            .WithName(nameof(SetUserPermissionOverrides))
            .WithSummary("Reemplazar permisos individuales de usuario")
            .WithDescription("Reemplaza el conjunto de excepciones/anulaciones (overrides) de permisos directamente asignados a un usuario.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid userId,
        [FromBody] SetUserPermissionOverridesRequest request,
        ICommandHandler<SetUserPermissionOverridesCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new SetUserPermissionOverridesCommand(userId, request.PermissionKeys);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record SetUserPermissionOverridesRequest(List<string> PermissionKeys);
