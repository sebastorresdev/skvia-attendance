using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.UpdateUser;

/// <summary>
/// Comando para actualizar la información de un usuario.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.Update)]
public record UpdateUserCommand(
    Guid UserId,
    string UserName,
    bool IsActive,
    string Email,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<Guid> RoleIds) : ICommand<ErrorOr<Success>>;



