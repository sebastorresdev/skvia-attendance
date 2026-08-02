namespace Skvia.Attendance.Application.Features.Roles.Commands.UpdateRole;

public record UpdateRoleCommand(Guid Id, string Name, string? Description) : ICommand<ErrorOr<Success>>;
