using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Users.Commands.DeleteUser;

/// <summary>
/// Comando para eliminar un usuario del sistema.
/// </summary>
[AuthorizeCommand(Permissions = Permission.User.Delete)]
public record DeleteUserCommand(List<Guid> UserIds) : ICommand<ErrorOr<Success>>;



