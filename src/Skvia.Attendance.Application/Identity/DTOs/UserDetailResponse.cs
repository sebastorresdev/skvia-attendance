namespace Skvia.Attendance.Application.Identity.DTOs;

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
