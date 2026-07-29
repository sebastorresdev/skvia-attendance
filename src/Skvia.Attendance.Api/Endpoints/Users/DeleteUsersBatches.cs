using Microsoft.AspNetCore.Mvc;

using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.Commands.DeleteUser;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class DeleteBatchUsers : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapDelete("/batch", Handle)
            .WithSummary("Eliminar Usuarios")
            .RequireAuthorization()
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemResponse>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> Handle(
        [FromBody] DeleteUserCommand command,
        ICommandHandler<DeleteUserCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
