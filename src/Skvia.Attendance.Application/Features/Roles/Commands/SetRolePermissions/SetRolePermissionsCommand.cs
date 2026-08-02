namespace Skvia.Attendance.Application.Features.Roles.Commands.SetRolePermissions;

public record SetRolePermissionsCommand(Guid RoleId, List<string> PermissionKeys) : ICommand<ErrorOr<Success>>;
