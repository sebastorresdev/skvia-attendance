namespace Skvia.Attendance.Api.Endpoints.Users.Requests;

public record SetUserPermissionOverridesRequest(List<string> PermissionKeys);
