namespace Skvia.Attendance.Application.Features.Users.DTOs;

public record UserDetailResponse(
    Guid UserId,
    string? DisplayName,
    string UserName,
    bool IsActive,
    List<Guid> BranchIds,
    List<Guid> RoleIds,
    string? Email,
    string? PhotoUrl,
    string? PhoneNumber,
    DateTime CreatedAt,
    DateTime LastModifiedAt
);
