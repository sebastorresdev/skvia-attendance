using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Employees.DTOs;
using Skvia.Attendance.Domain.Employees;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmployeeByIdQuery, ErrorOr<EmployeeDetailResponse>>
{
    public async Task<ErrorOr<EmployeeDetailResponse>> HandleAsync(GetEmployeeByIdQuery query, CancellationToken cancellationToken)
    {
        var employee = await (
            from e in dbContext.Employees.AsNoTracking()
            where e.Id == query.EmployeeId
            join b in dbContext.Branches.AsNoTracking() on e.MainBranchId equals b.Id into branchGroup
            from b in branchGroup.DefaultIfEmpty()
            select new EmployeeDetailResponse(
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
                e.HireDate,
                e.PhotoUrl,
                e.MainBranchId,
                b != null ? b.Name : null,
                e.Status,
                e.MobileCheckInEnabled,
                e.ApplicationUserId,
                e.RequireFourPointAttendance,
                e.IsAttendanceTracked,
                e.AutoCompleteClockOut,
                e.TardinessToleranceMinutes,
                e.AllowedWorkplaceIds)
        ).FirstOrDefaultAsync(cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }

        return employee;
    }
}
