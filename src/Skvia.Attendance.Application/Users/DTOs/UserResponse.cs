namespace Skvia.Attendance.Application.Users.DTOs;

public record UserResponse(
    Guid UserId,
    string UserName,
    bool IsActive,
    string BranchName,
    List<string> RoleNames,
    string? Email,
    string? PhotoUrl,
    DateTime LastModifiedAt
);
