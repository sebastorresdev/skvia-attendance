using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;
using Skvia.Attendance.Domain.EmployeeSchedules;

using ErrorOr;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetEmployeeSchedule;

public class GetEmployeeScheduleQueryHandler(IApplicationDbContext context)
    : IQueryHandler<GetEmployeeScheduleQuery, ErrorOr<List<EmployeeScheduleResponse>>>
{
    public async Task<ErrorOr<List<EmployeeScheduleResponse>>> HandleAsync(GetEmployeeScheduleQuery request, CancellationToken cancellationToken)
    {
        return await context.EmployeeSchedules
            .AsNoTracking()
            .Include(x => x.Branch)
            .Where(es => es.EmployeeId == request.EmployeeId && es.Date >= request.StartDate && es.Date <= request.EndDate)
            .OrderBy(es => es.Date)
            .Select(es => new EmployeeScheduleResponse(
                es.Id,
                es.EmployeeId,
                es.Date,
                es.BranchId,
                es.Branch.Name,
                es.AssignedStartTime,
                es.AssignedEndTime,
                es.DayType,
                es.BaseScheduleId))
            .ToListAsync(cancellationToken);
    }
}
