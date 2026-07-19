namespace Skvia.Attendance.Contracts.Users;

public record CreateUserRequest(
        string UserName,
        string Password,
        string Email,
        string? DisplayName,
        string? PhoneNumber,
        string? PhotoUrl,
        List<string> BranchIds,
        List<string> Roles
    );
