namespace Skvia.Attendance.Application.Identity.Commands.CreateUser;

public record CreateUserCommand(
    string UserName,
    string Password,
    string? DisplayName,
    string? Email,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<Guid> RoleIds
) : ICommand<ErrorOr<Guid>>;
