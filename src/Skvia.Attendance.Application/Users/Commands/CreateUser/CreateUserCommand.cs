namespace Skvia.Attendance.Application.Users.Commands.CreateUser;

public record CreateUserCommand(
    string UserName,
    string Password,
    string Email,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<string> Roles
) : ICommand<ErrorOr<Guid>>;
