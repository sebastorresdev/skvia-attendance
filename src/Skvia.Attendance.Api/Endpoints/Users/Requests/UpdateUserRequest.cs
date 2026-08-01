namespace Skvia.Attendance.Api.Endpoints.Users.Requests;

public record UpdateUserRequest(
    string UserName,
    bool IsActive,
    string Email,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<Guid> RoleIds);
