namespace Skvia.Attendance.Contracts.Users;

public record ResetPasswordRequest(string UserId, string NewPassword, string ConfirmNewPassword);
