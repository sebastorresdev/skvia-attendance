using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Features.Branches.DTOs;
using Skvia.Erp.Domain.Branches;

namespace Skvia.Erp.Application.Features.Branches.Queries.GetBranchById;

public class GetBranchByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetBranchByIdQuery, ErrorOr<BranchDetailResponse>>
{
    public async Task<ErrorOr<BranchDetailResponse>> HandleAsync(GetBranchByIdQuery query, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches
            .AsNoTracking()
            .Where(b => b.Id == query.BranchId)
            .Select(b => new BranchDetailResponse(b.Id, b.Code, b.Name, b.Address))
            .FirstOrDefaultAsync(cancellationToken);

        return branch is not null
            ? branch
            : BranchErrors.NotFound;
    }
}


