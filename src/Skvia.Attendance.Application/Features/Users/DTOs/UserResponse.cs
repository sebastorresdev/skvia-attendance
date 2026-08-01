namespace Skvia.Attendance.Application.Features.Users.DTOs;

public record UserResponse(
    Guid Id,
    string UserName,
    bool IsActive,
    string BranchName,
    List<string> RoleNames,
    string? Email,
    string? PhotoUrl,
    DateTime LastModifiedAt
);
