using Microsoft.AspNetCore.Mvc;

using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Roles.Commands.DeleteRole;

namespace Skvia.Attendance.Api.Endpoints.Roles;

public class DeleteBatchRoles : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/batch", Handle)
            .WithName(nameof(DeleteBatchRoles))
            .WithSummary("Eliminar roles en lote")
            .WithDescription("Elimina múltiples roles del sistema recibiendo una lista de sus identificadores en el cuerpo de la petición.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        [FromBody] DeleteRolesBatchRequest request,
        ICommandHandler<DeleteRoleCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRoleCommand(request.RoleIds);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
public record DeleteRolesBatchRequest(List<Guid> RoleIds);
