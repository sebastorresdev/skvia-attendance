namespace Skvia.Attendance.Application.Identity.DTOs;

public record UserResponse(
    string UserName,
    bool IsActive,
    List<string>? BranchNames,
    List<string>? RoleNames,
    string? Email,
    string? PhotoUrl,
    DateTime? LastModifiedAt
);
