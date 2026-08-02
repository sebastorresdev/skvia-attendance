namespace Skvia.Attendance.Api.Endpoints.Roles.Requests;

public class SetRolePermissionsRequest
{
    public List<string> PermissionKeys { get; set; } = [];
}
