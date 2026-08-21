using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Departments.DTOs;

using Microsoft.EntityFrameworkCore;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Departments.Queries.GetDepartments;

public class GetDepartmentsQueryHandler(IApplicationDbContext dbContext) 
    : IQueryHandler<GetDepartmentsQuery, ErrorOr<List<DepartmentResponse>>>
{
    public async Task<ErrorOr<List<DepartmentResponse>>> HandleAsync(
        GetDepartmentsQuery query, 
        CancellationToken cancellationToken)
    {
        return await dbContext.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentResponse(d.Id, d.Name, d.Description))
            .ToListAsync(cancellationToken);
    }
}


