namespace Skvia.Erp.Application.Features.Users.DTOs;

public record UserDetailResponse(
    Guid Id,
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

