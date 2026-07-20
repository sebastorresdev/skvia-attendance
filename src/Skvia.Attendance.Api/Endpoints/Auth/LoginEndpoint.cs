using System.Security.Claims;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

using Skvia.Attendance.Application.Auth.Commands.Login;
using Skvia.Attendance.Contracts.Auth;

namespace Skvia.Attendance.Api.Endpoints.Auth;

public class LoginEndpoint : IEndpoint
{
   
    public static void Map(RouteGroupBuilder group)
        => group.MapPost("/login", Handle)
             .WithSummary("Iniciar sesión de usuario")
             .AllowAnonymous()
             .Produces<SignInHttpResult>();

    private static async Task<IResult> Handle(
        LoginRequest request,
        ICommandHandler<LoginCommand, ErrorOr<ClaimsPrincipal>> handler,
        CancellationToken ct)
    {
        var command = new LoginCommand(request.UserName, request.Password);

        var result = await handler.HandleAsync(command, ct);

        return result.Match(
            principal => TypedResults.SignIn(principal, authenticationScheme: IdentityConstants.BearerScheme),
            ResultExtensions.ToProblem);
    }
}
