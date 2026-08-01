using System.Security.Claims;

namespace Skvia.Attendance.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<LoginCommand, ErrorOr<ClaimsPrincipal>>
{
    public Task<ErrorOr<ClaimsPrincipal>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
        => userAccountService.AuthenticateAsync(command, cancellationToken);
}
