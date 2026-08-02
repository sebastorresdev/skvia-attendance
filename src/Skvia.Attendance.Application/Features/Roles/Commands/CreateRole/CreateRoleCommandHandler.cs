namespace Skvia.Attendance.Application.Features.Roles.Commands.CreateRole;

public class CreateRoleCommandHandler(IRoleService identityRoleService) : ICommandHandler<CreateRoleCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateRoleCommand command, CancellationToken cancellationToken)
        => await identityRoleService.CreateRoleAsync(command, cancellationToken);
}
