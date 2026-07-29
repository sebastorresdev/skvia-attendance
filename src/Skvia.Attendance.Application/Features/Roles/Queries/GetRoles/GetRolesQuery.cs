namespace Skvia.Attendance.Application.Features.Roles.Queries.GetRoles;

public record GetRolesQuery() : IQuery<ErrorOr<List<RoleResponse>>>;
