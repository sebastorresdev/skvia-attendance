namespace Skvia.Attendance.Application.Roles.Queries.GetRoles;

public record GetRolesQuery() : IQuery<ErrorOr<List<RoleResponse>>>;
