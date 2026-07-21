using Skvia.Attendance.Application.Users.Commands.ResetPassword;
using Skvia.Attendance.Contracts.Users;

namespace Skvia.Attendance.Api.Endpoints.Users;

public class ResetPassword : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
    {
        group.MapPost("/reset-password", Handle)
            .WithSummary("Reseto de contraseña. Solo el usuario con rol admin puede realizar esta operación");
    }

    private static async Task<IResult> Handle(
        ResetPasswordRequest request,
        ICommandHandler<ResetPasswordCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(request.UserName, request.NewPassword, request.ConfirmNewPassword);

        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            ResultExtensions.ToProblem);
    }
}
