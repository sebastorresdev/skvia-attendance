namespace Skvia.Attendance.Application.Features.Roles.Commands.DeleteRole;

public record DeleteRoleCommand(List<Guid> RoleIds) : ICommand<ErrorOr<Success>>;
