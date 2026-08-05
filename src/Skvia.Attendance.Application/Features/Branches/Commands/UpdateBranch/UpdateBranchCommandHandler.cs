using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Application.Features.Branches.Commands.UpdateBranch;

public class UpdateBranchCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<UpdateBranchCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateBranchCommand command, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.FindAsync([command.BranchId], cancellationToken);

        if (branch is null)
        {
            return BranchErrors.NotFound;
        }

        var cleanNormalizedCode = command.Code.Trim().ToUpperInvariant();

        if (await dbContext.Branches
               .AnyAsync(b => b.Code == cleanNormalizedCode && b.Id != command.BranchId, cancellationToken))
            return BranchErrors.DuplicateBranch(command.Name);

        branch.Update(cleanNormalizedCode, command.Name, command.Address, tardinessToleranceMinutes: command.TardinessToleranceMinutes);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
