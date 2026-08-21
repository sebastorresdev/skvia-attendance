using Skvia.Erp.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Branches;

namespace Skvia.Erp.Application.Features.Branches.Commands.UnarchiveBranch;

public class UnarchiveBranchCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UnarchiveBranchCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UnarchiveBranchCommand command, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.FindAsync([command.BranchId], cancellationToken);

        if (branch is null)
            return BranchErrors.NotFound;

        // TODO: Corregir implementacion
        // branch.Unarchive();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


