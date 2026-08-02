namespace Skvia.Attendance.Application.Features.Roles.Commands.CreateRole;

public record CreateRoleCommand(Guid Id, string Name, string? Description) : ICommand<ErrorOr<Guid>>;
