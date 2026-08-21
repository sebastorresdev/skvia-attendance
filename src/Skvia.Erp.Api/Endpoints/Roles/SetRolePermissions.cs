using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Roles.Commands.SetRolePermissions;

namespace Skvia.Erp.Api.Endpoints.Roles;

public class SetRolePermissions : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}/permissions", handle)
            .WithName(nameof(SetRolePermissions))
            .WithSummary("Establecer permisos de un rol")
            .WithDescription("Actualiza la lista completa de permisos asignados a un rol específico.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> handle(
        Guid id,
        SetRolePermissionsRequest request,
        ICommandHandler<SetRolePermissionsCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new SetRolePermissionsCommand(id, request.PermissionKeys);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public class SetRolePermissionsRequest
{
    public List<string> PermissionKeys { get; set; } = [];
}



