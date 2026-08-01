using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.Commands.UpdateUser;

namespace Skvia.Attendance.Api.Endpoints.Users;

public sealed class UpdateUser : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{userId:guid}", Handle)
            .WithName(nameof(UpdateUser))
            .WithSummary("Actualizar usuario")
            .WithDescription("Actualiza la información de un usuario existente.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ApiProblemDetails>(StatusCodes.Status409Conflict);

    private static async Task<IResult> Handle(
        Guid userId,
        UpdateUserCommand command,
        ICommandHandler<UpdateUserCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var commandWithUserId = command with { UserId = userId };

        var result = await handler.HandleAsync(commandWithUserId, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
