using Microsoft.AspNetCore.Mvc;

using Skvia.Attendance.Api.Endpoints.Users.Requests;
using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.Commands.DeleteUser;

namespace Skvia.Attendance.Api.Endpoints.Users;

public sealed class DeleteBatchUsers : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/batch", Handle)
            .WithName(nameof(DeleteBatchUsers))
            .WithSummary("Eliminar usuarios en lote")
            .WithDescription("Elimina múltiples usuarios del sistema recibiendo una lista de sus identificadores en el cuerpo de la petición.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        [FromBody] DeleteUsersBatchRequest request,
        ICommandHandler<DeleteUserCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(request.UserIds);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
