using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Roles.Commands.DeleteRole;

/// <summary>
/// Comando para eliminar un rol de sistema.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Role.Delete)]
public record DeleteRoleCommand(List<Guid> RoleIds) : ICommand<ErrorOr<Success>>;



