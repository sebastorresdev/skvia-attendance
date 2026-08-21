using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Roles.Commands.UpdateRole;

/// <summary>
/// Comando para actualizar un rol existente.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Role.Update)]
public record UpdateRoleCommand(Guid Id, string Name, string? Description) : ICommand<ErrorOr<Success>>;



