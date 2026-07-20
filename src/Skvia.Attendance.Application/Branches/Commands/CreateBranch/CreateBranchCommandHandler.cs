using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Application.Branches.Commands.CreateBranch;

public class CreateBranchCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<CreateBranchCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateBranchCommand command, CancellationToken cancellationToken)
    {
        var cleanCode = command.Code.Trim().ToUpperInvariant();

        var branchExisting = await dbContext.Branches
            .AnyAsync(b => b.Code == cleanCode, cancellationToken);

        if (branchExisting)
            return BranchErrors.DuplicateBranch(command.Code);

        var branch = Branch.Create(command.Code, command.Name, command.Address);

        dbContext.Branches.Add(branch);
        await dbContext.SaveChangesAsync(cancellationToken);

        return branch.Id;
    }
}
