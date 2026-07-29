namespace Skvia.Attendance.Application.Common.Models;

public record CurrentUser(
    Guid UserId,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
