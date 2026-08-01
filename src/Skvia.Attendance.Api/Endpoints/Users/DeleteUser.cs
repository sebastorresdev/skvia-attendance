using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.Commands.DeleteUser;

namespace Skvia.Attendance.Api.Endpoints.Users;

public sealed class DeleteUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapDelete("/{userId:guid}", Handle)
            .WithName(nameof(DeleteUser))
            .WithSummary("Eliminar usuario")
            .WithDescription("Elimina un usuario del sistema por su identificador único.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid userId,
        ICommandHandler<DeleteUserCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand([userId]);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
