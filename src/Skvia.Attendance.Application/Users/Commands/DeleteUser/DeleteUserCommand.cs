namespace Skvia.Attendance.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(List<Guid> UserIds) : ICommand<ErrorOr<Success>>;
