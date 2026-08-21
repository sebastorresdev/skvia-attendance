using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Branches.Commands.DeleteBranch;

public record DeleteBranchCommand(Guid BranchId) : ICommand<ErrorOr<Success>>;


