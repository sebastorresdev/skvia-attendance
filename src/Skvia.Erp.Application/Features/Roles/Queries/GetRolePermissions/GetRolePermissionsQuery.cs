using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.DTOs;

namespace Skvia.Erp.Application.Features.Roles.Queries.GetRolePermissions;

/// <summary>
/// Consulta para obtener los permisos asociados a un rol.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Role.View)]
public record GetRolePermissionsQuery(Guid RoleId) : IQuery<ErrorOr<List<PermissionGroupResponse>>>;



