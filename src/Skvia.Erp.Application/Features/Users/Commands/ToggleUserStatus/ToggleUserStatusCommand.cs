using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.ToggleUserStatus;

/// <summary>
/// Comando para activar o desactivar un usuario.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.Update)]
public record ToggleUserStatusCommand(
    Guid UserId,
    bool IsActive) : ICommand<ErrorOr<Success>>;



