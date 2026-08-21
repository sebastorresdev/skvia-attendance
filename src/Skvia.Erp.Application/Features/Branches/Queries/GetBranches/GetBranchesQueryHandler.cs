using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Features.Branches.DTOs;

namespace Skvia.Erp.Application.Features.Branches.Queries.GetBranches;

/// <summary>
/// Manejador para la consulta de obtención del listado de sedes/sucursales.
/// </summary>
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


