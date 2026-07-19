namespace Skvia.Attendance.Application.Common.Models;

public record UserResponse(
    string UserName,
    bool IsActive,
    List<string>? BranchNames,
    List<string>? RoleNames,
    string? Email,
    string? PhotoUrl,
    DateTime? LastModifiedAt
);
