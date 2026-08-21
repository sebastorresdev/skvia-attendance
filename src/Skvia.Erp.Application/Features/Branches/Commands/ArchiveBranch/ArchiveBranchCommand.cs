using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
namespace Skvia.Erp.Application.Features.Branches.Commands.ArchiveBranch;

public record ArchiveBranchCommand(Guid BranchId) : ICommand<ErrorOr<Success>>;


