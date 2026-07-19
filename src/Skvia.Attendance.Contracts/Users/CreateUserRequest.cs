namespace Skvia.Attendance.Contracts.Users;

public record CreateUserRequest(
        string UserName,
        string Password,
        string? DisplayName,
        string? Email,
        string? PhoneNumber,
        string? PhotoUrl,
        List<string> BranchIds,
        List<string> RoleIds
    );
