using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Roles.Commands.DeleteRole;

namespace Skvia.Erp.Api.Endpoints.Roles;

public class DeleteRole : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{roleId:guid}", Handle)
            .WithName(nameof(DeleteRole))
            .WithSummary("Eliminar rol")
            .WithDescription("Elimina un rol del sistema por su identificador único.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid roleId,
        ICommandHandler<DeleteRoleCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteRoleCommand([roleId]);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}



