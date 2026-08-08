using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Api.Models;
using Skvia.Attendance.Application.Features.Auth.Commands.Login;

namespace Skvia.Attendance.Api.Endpoints.Auth;

public sealed class Login : IEndpoint
{
    private sealed record AuthTokenResponse(
        string TokenType,
        string AccessToken,
        int ExpiresIn,
        string RefreshToken);

    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/login", Handle)
            .WithName(nameof(Login))
            .WithSummary("Iniciar sesión")
            .WithDescription("Autentica a un usuario con sus credenciales y genera un token de acceso.")
            .AllowAnonymous()
            .RequireRateLimiting("StrictLogin")
            .Produces<AuthTokenResponse>(StatusCodes.Status200OK)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status401Unauthorized);

    private static async Task<IResult> Handle(
        LoginRequest request,
        ICommandHandler<LoginCommand, ErrorOr<ClaimsPrincipal>> handler,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.UserName, request.Password);
        var result = await handler.HandleAsync(command, cancellationToken);

        return result.Match(
            principal => TypedResults.SignIn(principal, authenticationScheme: IdentityConstants.BearerScheme),
            errors => errors.ToProblem());
    }
}

public record LoginRequest(string UserName, string Password);
