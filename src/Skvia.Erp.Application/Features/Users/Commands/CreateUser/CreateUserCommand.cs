using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.CreateUser;

/// <summary>
/// Comando para registrar un nuevo usuario en el sistema.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.Create)]
public record CreateUserCommand(
    string UserName,
    string Password,
    string Email,
    string? DisplayName,
    string? PhoneNumber,
    string? PhotoUrl,
    List<Guid> BranchIds,
    List<Guid> RoleIds
) : ICommand<ErrorOr<Guid>>;



