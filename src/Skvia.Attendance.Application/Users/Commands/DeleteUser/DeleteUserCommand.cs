namespace Skvia.Attendance.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid UserId) : ICommand<ErrorOr<Success>>;
