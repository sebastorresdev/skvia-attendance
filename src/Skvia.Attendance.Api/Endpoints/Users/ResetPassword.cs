using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Users.Commands.ResetPassword;

namespace Skvia.Attendance.Api.Endpoints.Users;

public sealed class ResetPassword : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/reset-password", Handle)
            .WithName(nameof(ResetPassword))
            .WithSummary("Restablecer contraseña")
            .WithDescription("Restablece la contraseña de un usuario. Requiere permisos de administración.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        ResetPasswordCommand command,
        ICommandHandler<ResetPasswordCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}
