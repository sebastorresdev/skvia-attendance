namespace Skvia.Attendance.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(
    Guid UserId,
    string UserName,
    bool IsActive,
    string Email,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<string> Roles) : ICommand<ErrorOr<Success>>;
