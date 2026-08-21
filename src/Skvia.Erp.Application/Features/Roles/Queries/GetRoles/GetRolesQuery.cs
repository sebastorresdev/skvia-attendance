using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.Roles.DTOs;

namespace Skvia.Erp.Application.Features.Roles.Queries.GetRoles;

/// <summary>
/// Consulta para obtener el listado de roles del sistema.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Role.View)]
public record GetRolesQuery() : IQuery<ErrorOr<List<RoleResponse>>>;



