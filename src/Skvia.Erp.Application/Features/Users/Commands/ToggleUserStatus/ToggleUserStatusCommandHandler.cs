using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<ToggleUserStatusCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(ToggleUserStatusCommand command, CancellationToken cancellationToken)
        => userAccountService.ToggleUserStatusAsync(command, cancellationToken);
}


