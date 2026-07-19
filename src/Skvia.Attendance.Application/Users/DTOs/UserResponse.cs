namespace Skvia.Attendance.Application.Users.DTOs;

public record UserResponse(
    Guid UserId,
    string UserName,
    bool IsActive,
    List<string>? BranchNames,
    List<string>? RoleNames,
    string? Email,
    string? PhotoUrl,
    DateTime? LastModifiedAt
);
