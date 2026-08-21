using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Roles.Commands.SetRolePermissions;

public class SetRolePermissionsCommandHandler(IRoleService roleService) : ICommandHandler<SetRolePermissionsCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(SetRolePermissionsCommand command, CancellationToken cancellationToken)
        => roleService.SetRolePermissionsAsync(command, cancellationToken);
}


