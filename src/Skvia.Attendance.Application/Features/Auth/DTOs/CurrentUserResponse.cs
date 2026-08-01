namespace Skvia.Attendance.Application.Features.Auth.DTOs;

public record CurrentUserResponse(
    Guid UserId,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
