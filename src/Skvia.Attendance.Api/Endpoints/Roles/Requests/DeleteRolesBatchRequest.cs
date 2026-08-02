namespace Skvia.Attendance.Api.Endpoints.Roles.Requests;

public record DeleteRolesBatchRequest(List<Guid> RoleIds);
