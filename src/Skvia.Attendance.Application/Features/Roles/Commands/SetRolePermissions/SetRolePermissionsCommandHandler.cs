using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Roles.Commands.SetRolePermissions;

public class SetRolePermissionsCommandHandler(IRoleService roleService) : ICommandHandler<SetRolePermissionsCommand, ErrorOr<Success>>
{
    public Task<ErrorOr<Success>> HandleAsync(SetRolePermissionsCommand command, CancellationToken cancellationToken)
        => roleService.SetRolePermissionsAsync(command, cancellationToken);
}
