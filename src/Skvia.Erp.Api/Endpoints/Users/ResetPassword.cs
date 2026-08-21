using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Users.Commands.ResetPassword;

namespace Skvia.Erp.Api.Endpoints.Users;

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
        ResetPasswordRequest request,
        ICommandHandler<ResetPasswordCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(request.UserId, request.NewPassword, request.ConfirmNewPassword);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record ResetPasswordRequest(string UserId, string NewPassword, string ConfirmNewPassword);



