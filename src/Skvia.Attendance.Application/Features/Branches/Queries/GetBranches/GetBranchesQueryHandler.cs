using Skvia.Attendance.Application.Features.Branches.DTOs;

namespace Skvia.Attendance.Application.Features.Branches.Queries.GetBranches;

public class GetBranchesQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetBranchesQuery, ErrorOr<List<BranchResponse>>>
{
    public async Task<ErrorOr<List<BranchResponse>>> HandleAsync(GetBranchesQuery query, CancellationToken cancellationToken)
    {
        return await dbContext.Branches
            .AsNoTracking()
            .Select(b => new BranchResponse(b.Id, b.Code, b.Name, b.Address))
            .ToListAsync(cancellationToken);
    }
}
