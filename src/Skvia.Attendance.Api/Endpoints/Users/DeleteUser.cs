using Skvia.Attendance.Application.Features.Users.Commands.DeleteUser;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class DeleteUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/{userId:guid}", Handle)
            .WithSummary("Eliminar Usuario")
            .RequireAuthorization();
    }

    private static async Task<IResult> Handle(
        Guid userId,
        ICommandHandler<DeleteUserCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand([userId]);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
