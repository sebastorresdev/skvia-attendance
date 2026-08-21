using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Roles.Commands.SetRolePermissions;

/// <summary>
/// Comando para establecer los permisos asignados a un rol.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Role.Update)]
public record SetRolePermissionsCommand(Guid RoleId, List<string> PermissionKeys) : ICommand<ErrorOr<Success>>;



