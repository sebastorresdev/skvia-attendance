using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Employees.DTOs;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmployeesQuery, ErrorOr<List<EmployeeResponse>>>
{
    public async Task<ErrorOr<List<EmployeeResponse>>> HandleAsync(GetEmployeesQuery query, CancellationToken cancellationToken)
    {
        var employees = await (
            from e in dbContext.Employees.AsNoTracking()
            join b in dbContext.Branches.AsNoTracking() on e.MainBranchId equals b.Id into branchGroup
            from b in branchGroup.DefaultIfEmpty()
            orderby e.Code
            select new EmployeeResponse(
                e.Id,
                e.Code,
                e.FirstName,
                e.LastName,
                e.DocumentIdentifier.Type,
                e.DocumentIdentifier.Number,
                e.Email.HasValue ? e.Email.Value.Value : null,
                e.Phone.HasValue ? e.Phone.Value.Value : null,
                e.DepartmentId,
                e.Position,
                e.PhotoUrl,
                e.MainBranchId,
                b != null ? b.Name : null,
                e.Status,
                e.MobileCheckInEnabled,
                e.ApplicationUserId
            )
        ).ToListAsync(cancellationToken);

        return employees;
    }
}
