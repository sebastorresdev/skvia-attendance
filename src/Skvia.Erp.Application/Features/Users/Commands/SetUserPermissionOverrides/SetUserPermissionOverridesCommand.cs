using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.SetUserPermissionOverrides;

/// <summary>
/// Comando para configurar sobreescritura de permisos directos a un usuario.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.Update)]
public record SetUserPermissionOverridesCommand(
    Guid UserId,
    List<string> PermissionKeys
) : ICommand<ErrorOr<Success>>;



