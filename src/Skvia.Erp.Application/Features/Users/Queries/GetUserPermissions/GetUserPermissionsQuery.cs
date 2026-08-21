using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.DTOs;

namespace Skvia.Erp.Application.Features.Users.Queries.GetUserPermissions;

/// <summary>
/// Consulta para obtener los permisos asignados a un usuario.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.View)]
public record GetUserPermissionsQuery(Guid UserId) : IQuery<ErrorOr<List<PermissionGroupResponse>>>;




