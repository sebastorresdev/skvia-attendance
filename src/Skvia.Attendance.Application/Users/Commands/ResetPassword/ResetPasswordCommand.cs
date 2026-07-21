namespace Skvia.Attendance.Application.Users.Commands.ResetPassword;

public record ResetPasswordCommand(string UserId, string NewPassword, string ConfirmNewPassword) : ICommand<ErrorOr<Success>>;
