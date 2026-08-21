using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.SetUserPermissionOverrides;

public class SetUserPermissionOverridesCommandHandler(IUserAccountService userAccountService)
    : ICommandHandler<SetUserPermissionOverridesCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(SetUserPermissionOverridesCommand command, CancellationToken cancellationToken)
        => userAccountService.SetPermissionOverridesAsync(command, cancellationToken);
}


