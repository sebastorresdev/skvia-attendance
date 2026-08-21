using Skvia.Erp.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Branches;

namespace Skvia.Erp.Application.Features.Branches.Commands.ArchiveBranch;

public class ArchiveBranchCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<ArchiveBranchCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(ArchiveBranchCommand command, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.FindAsync([command.BranchId], cancellationToken);

        if (branch is null)
            return BranchErrors.NotFound;

        // TODO: Corregir funcionalidad
        // branch.Archive();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


