using Skvia.Erp.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Branches;

namespace Skvia.Erp.Application.Features.Branches.Commands.DeleteBranch;

public class DeleteBranchCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<DeleteBranchCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteBranchCommand command, CancellationToken cancellationToken)
    {
        var affectedRows = await dbContext.Branches
            .Where(u => u.Id == command.BranchId)
            .ExecuteDeleteAsync(cancellationToken);

        if (affectedRows == 0)
        {
            return BranchErrors.NotFound;
        }

        return Result.Success;
    }
}


