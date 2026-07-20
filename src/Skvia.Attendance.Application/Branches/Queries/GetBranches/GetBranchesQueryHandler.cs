using Skvia.Attendance.Application.Branches.DTOs;

namespace Skvia.Attendance.Application.Branches.Queries.GetBranches;

public class GetBranchesQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetBranchesQuery, ErrorOr<List<GetBranchResult>>>
{
    public async Task<ErrorOr<List<GetBranchResult>>> HandleAsync(GetBranchesQuery query, CancellationToken cancellationToken)
    {
        var branches = await dbContext.Branches
            .AsNoTracking()
            .Select(b => new GetBranchResult(b.Id, b.Code, b.Name, b.Address))
            .ToListAsync(cancellationToken);

        return branches;
    }
}
