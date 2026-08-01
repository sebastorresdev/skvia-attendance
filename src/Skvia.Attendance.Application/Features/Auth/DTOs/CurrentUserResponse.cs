namespace Skvia.Attendance.Application.Features.Auth.DTOs;

public record CurrentUserResponse(
    Guid Id,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions);
