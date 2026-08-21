using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Features.Justifications.DTOs;

namespace Skvia.Erp.Application.Features.Justifications.Queries.GetJustifications;

public class GetJustificationsQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetJustificationsQuery, ErrorOr<List<JustificationResponse>>>
{
    public async Task<ErrorOr<List<JustificationResponse>>> HandleAsync(GetJustificationsQuery query, CancellationToken cancellationToken)
    {
        var queryable = dbContext.Justifications
            .AsNoTracking();

        if (query.StartDate.HasValue)
        {
            queryable = queryable.Where(j => j.Date >= query.StartDate.Value);
        }

        if (query.EndDate.HasValue)
        {
            queryable = queryable.Where(j => j.Date <= query.EndDate.Value);
        }

        if (query.EmployeeId.HasValue)
        {
            queryable = queryable.Where(j => j.EmployeeId == query.EmployeeId.Value);
        }

        if (query.Status.HasValue)
        {
            queryable = queryable.Where(j => j.Status == query.Status.Value);
        }

        if (query.BranchId.HasValue)
        {
            queryable = queryable.Where(j => j.Employee.MainBranchId == query.BranchId.Value);
        }

        var list = await queryable
            .OrderByDescending(j => j.Created)
            .Select(j => new JustificationResponse(
                j.Id,
                j.EmployeeId,
                $"{j.Employee.LastName}, {j.Employee.FirstName}",
                j.Employee.Code,
                j.Employee.MainBranchId.HasValue ? dbContext.Branches.Where(b => b.Id == j.Employee.MainBranchId.Value).Select(b => b.Name).FirstOrDefault() ?? "N/A" : "N/A",
                j.Date,
                j.Type,
                j.Reason,
                j.DocumentUrl,
                j.Status,
                j.ReviewerNotes,
                j.ReviewedAt,
                j.Created
            ))
            .ToListAsync(cancellationToken);

        return list;
    }
}

