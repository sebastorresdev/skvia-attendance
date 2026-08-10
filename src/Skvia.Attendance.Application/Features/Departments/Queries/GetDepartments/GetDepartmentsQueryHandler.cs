using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Departments.DTOs;

using Microsoft.EntityFrameworkCore;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Departments.Queries.GetDepartments;

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
