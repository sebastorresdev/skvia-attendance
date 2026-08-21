using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Branches.Commands.UnarchiveBranch;

public record UnarchiveBranchCommand(Guid BranchId) : ICommand<ErrorOr<Success>>;


