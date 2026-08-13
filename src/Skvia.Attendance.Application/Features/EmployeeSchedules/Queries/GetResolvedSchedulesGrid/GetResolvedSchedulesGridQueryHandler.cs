using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetResolvedSchedulesGrid;

public class GetResolvedSchedulesGridQueryHandler(
    IApplicationDbContext dbContext,
    IScheduleResolverService resolverService)
    : IQueryHandler<GetResolvedSchedulesGridQuery, ErrorOr<List<EmployeeScheduleGridRowDto>>>
{
    public async Task<ErrorOr<List<EmployeeScheduleGridRowDto>>> HandleAsync(
        GetResolvedSchedulesGridQuery request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Employees
            .AsNoTracking()
            .Where(e => e.Status == EmployeeStatus.Active && e.IsAttendanceTracked);

        if (request.BranchId.HasValue)
        {
            query = query.Where(e => e.MainBranchId == request.BranchId.Value);
        }

        if (request.DepartmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == request.DepartmentId.Value);
        }

        var employees = await query
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToListAsync(cancellationToken);

        if (employees.Count == 0)
            return new List<EmployeeScheduleGridRowDto>();

        var departments = await dbContext.Departments.AsNoTracking().ToDictionaryAsync(d => d.Id, d => d.Name, cancellationToken);
        var branches = await dbContext.Branches.AsNoTracking().ToDictionaryAsync(b => b.Id, b => b.Name, cancellationToken);

        var employeeIds = employees.Select(e => e.Id).ToList();

        var grid = await resolverService.ResolveGridAsync(
            employeeIds,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        var rows = employees.Select(e => new EmployeeScheduleGridRowDto(
            EmployeeId: e.Id,
            EmployeeName: $"{e.FirstName} {e.LastName}",
            EmployeeCode: e.Code,
            DepartmentName: e.DepartmentId.HasValue && departments.TryGetValue(e.DepartmentId.Value, out var deptName) ? deptName : null,
            BranchName: e.MainBranchId.HasValue && branches.TryGetValue(e.MainBranchId.Value, out var branchName) ? branchName : null,
            Days: grid.TryGetValue(e.Id, out var days) ? days : []
        )).ToList();

        return rows;
    }
}
