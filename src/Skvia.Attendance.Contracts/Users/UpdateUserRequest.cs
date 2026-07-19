namespace Skvia.Attendance.Contracts.Users;

public record UpdateUserRequest(
    string UserName,
    string Email,
    bool IsActive,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<string> BranchIds,
    List<string> Roles);
