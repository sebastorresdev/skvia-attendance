namespace Skvia.Attendance.Application.Features.Users.Commands.DeleteUser;

public record DeleteUserCommand(List<Guid> UserIds) : ICommand<ErrorOr<Success>>;
