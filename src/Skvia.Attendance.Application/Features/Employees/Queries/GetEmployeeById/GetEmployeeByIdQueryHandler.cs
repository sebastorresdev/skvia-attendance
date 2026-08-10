using Skvia.Attendance.Application.Features.Employees.DTOs;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmployeeByIdQuery, ErrorOr<EmployeeDetailResponse>>
{
    public async Task<ErrorOr<EmployeeDetailResponse>> HandleAsync(GetEmployeeByIdQuery query, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .Where(e => e.Id == query.EmployeeId)
            .Select(e => new EmployeeDetailResponse(
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
                e.HireDate,
                e.PhotoUrl,
                e.MainBranchId,
                e.MainBranchId.HasValue ? dbContext.Branches.Where(b => b.Id == e.MainBranchId.Value).Select(b => b.Name).FirstOrDefault() : null,
                e.Status,
                e.MobileCheckInEnabled,
                e.ApplicationUserId,
                e.RequireFourPointAttendance,
                e.IsAttendanceTracked,
                e.AutoCompleteClockOut,
                e.TardinessToleranceMinutes,
                e.AllowedWorkplaceIds))
            .FirstOrDefaultAsync(cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }

        return employee;
    }
}
