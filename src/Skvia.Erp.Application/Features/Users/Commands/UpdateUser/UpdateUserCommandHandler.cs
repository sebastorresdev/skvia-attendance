using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<UpdateUserCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
        => userAccountService.UpdateUserAsync(command, cancellationToken);
}


