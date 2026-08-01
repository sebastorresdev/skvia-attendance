namespace Skvia.Attendance.Api.Endpoints.Users.Requests;

public record CreateUserRequest(
    string UserName,
    string Password,
    string Email,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<Guid> RoleIds);
