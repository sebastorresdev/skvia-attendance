using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.Roles.DTOs;

namespace Skvia.Erp.Application.Features.Roles.Queries.GetRoleById;

/// <summary>
/// Consulta para obtener el detalle de un rol por ID.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Role.View)]
public record GetRoleByIdQuery(Guid Id) : IQuery<ErrorOr<RoleResponse>>;



