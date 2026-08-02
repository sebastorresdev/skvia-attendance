using Skvia.Attendance.Application.Features.Roles.DTOs;

namespace Skvia.Attendance.Application.Features.Roles.Queries.GetRoleById;

public record GetRoleByIdQuery(Guid Id) : IQuery<ErrorOr<RoleResponse>>;
