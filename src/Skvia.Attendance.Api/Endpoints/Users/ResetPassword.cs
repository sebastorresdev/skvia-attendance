using Skvia.Attendance.Application.Features.Users.Commands.ResetPassword;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class ResetPassword : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/reset-password", Handle)
            .WithSummary("Reseto de contraseña. Solo el usuario con rol admin puede realizar esta operación")
            .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> Handle(
        ResetPasswordCommand command,
        ICommandHandler<ResetPasswordCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
