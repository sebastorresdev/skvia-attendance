using Skvia.Attendance.Application.Users.Commands.UpdateUser;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class UpdateUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPut("/{userId:guid}", Handle)
            .WithSummary("Actualizar Usuario")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> Handle(
        Guid userId,
        UpdateUserCommand command,
        ICommandHandler<UpdateUserCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        if (userId != command.UserId)
            return TypedResults.BadRequest();

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
