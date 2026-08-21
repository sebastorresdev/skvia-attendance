using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Roles.Commands.CreateRole;

/// <summary>
/// Comando para crear un nuevo rol de sistema.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Role.Create)]
public record CreateRoleCommand(string Name, string? Description) : ICommand<ErrorOr<Guid>>;



