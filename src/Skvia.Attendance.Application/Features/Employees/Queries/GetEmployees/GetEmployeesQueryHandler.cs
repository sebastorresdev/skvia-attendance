using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Employees.DTOs;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmployeesQuery, ErrorOr<List<EmployeeResponse>>>
{
    public async Task<ErrorOr<List<EmployeeResponse>>> HandleAsync(GetEmployeesQuery query, CancellationToken cancellationToken)
    {
        var employees = await dbContext.Employees
            .AsNoTracking()
            .Select(e => new EmployeeResponse
            (
                e.Id,
                e.Code,
                e.FirstName,
                e.LastName,
                e.DocumentIdentifier.Type,
                e.DocumentIdentifier.Number,
                e.Email != null ? e.Email.Value.Value : null,
                e.Phone != null ? e.Phone.Value.Value : null,
                e.DepartmentId,
                e.Position,
                e.PhotoUrl,
                e.MainBranchId,
                e.MainBranchId.HasValue ? dbContext.Branches.Where(b => b.Id == e.MainBranchId.Value).Select(b => b.Name).FirstOrDefault() : null,
                e.Status,
                e.MobileCheckInEnabled,
                e.ApplicationUserId
            ))
            .ToListAsync(cancellationToken);

        return employees;
    }
}
