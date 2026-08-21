using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
using System.Security.Claims;

namespace Skvia.Erp.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<LoginCommand, ErrorOr<ClaimsPrincipal>>
{
    public Task<ErrorOr<ClaimsPrincipal>> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
        => userAccountService.AuthenticateAsync(command, cancellationToken);
}


