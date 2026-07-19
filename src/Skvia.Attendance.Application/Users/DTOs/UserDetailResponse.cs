namespace Skvia.Attendance.Application.Users.DTOs;

public record UserDetailResponse(
    Guid UserId,
    string? DisplayName,
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
