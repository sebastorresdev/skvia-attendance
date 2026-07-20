using Skvia.Attendance.Application.Branches.DTOs;
using Skvia.Attendance.Domain.Branches;

namespace Skvia.Attendance.Application.Branches.Queries.GetBranchById;

public class GetBranchByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetBranchByIdQuery, ErrorOr<GetBranchByIdResult>>
{
    public async Task<ErrorOr<GetBranchByIdResult>> HandleAsync(GetBranchByIdQuery query, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches
            .AsNoTracking()
            .Where(b => b.Id == query.BranchId)
            .Select(b => new GetBranchByIdResult(b.Id, b.Code, b.Name, b.Address))
            .FirstOrDefaultAsync(cancellationToken);

        return branch is not null
            ? branch
            : BranchErrors.NotFound;
    }
}
