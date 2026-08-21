using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.ResetPassword;

public class ResetPasswordCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<ResetPasswordCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken)
        => userAccountService.ResetPasswordAsync(command, cancellationToken);
}


