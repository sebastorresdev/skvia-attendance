using ErrorOr;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.Branches.Commands.CreateBranch;

/// <summary>
/// Comando para crear una nueva sede o sucursal.
/// </summary>
[AuthorizeCommand(Permissions = Permission.Branch.Create)]
public record CreateBranchCommand(string Code, string Name, string? Address) : ICommand<ErrorOr<Guid>>;


