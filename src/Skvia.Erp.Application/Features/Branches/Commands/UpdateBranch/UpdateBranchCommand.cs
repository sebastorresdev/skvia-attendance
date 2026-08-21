using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Branches.Commands.UpdateBranch;

public record UpdateBranchCommand(Guid BranchId, string Code, string Name, string? Address) : ICommand<ErrorOr<Success>>;


