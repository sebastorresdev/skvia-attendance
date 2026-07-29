using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Features.Auth.Commands.Login;

namespace Skvia.Attendance.Api.Endpoints.Auth;

public class LoginEndpoint : IEndpoint
{
    private record AuthTokenResponse(string TokenType, string AccessToken, int ExpiresIn, string RefreshToken);
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/login", Handle)
             .WithSummary("Iniciar sesión de usuario")
             .AllowAnonymous()
             .Produces<AuthTokenResponse>();

    private static async Task<IResult> Handle(
        LoginCommand command,
        ICommandHandler<LoginCommand, ErrorOr<ClaimsPrincipal>> handler,
        CancellationToken ct)
    {
        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            principal => TypedResults.SignIn(principal, authenticationScheme: IdentityConstants.BearerScheme),
            ResultExtensions.ToProblem);
    }
}
