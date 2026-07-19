namespace Skvia.Attendance.Application.Common.Models;

public record UserDetailResponse(
    string DisplayName,
    string UserName,
    bool IsActive,
    List<string>? BranchNames,
    List<string>? RoleNames,
    string? Email,
    string? PhotoUrl,
    string? PhoneNumber,
    DateTime? CreatedAt,
    DateTime? LastModifiedAt
);
