namespace Skvia.Attendance.Application.Common.Models;

public record CurrentUser(
    Guid Id,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
