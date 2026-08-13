using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Queries.GetResolvedEmployeeSchedule;

public class GetResolvedEmployeeScheduleQueryHandler(IScheduleResolverService resolverService)
    : IQueryHandler<GetResolvedEmployeeScheduleQuery, ErrorOr<List<ResolvedScheduleDayDto>>>
{
    public async Task<ErrorOr<List<ResolvedScheduleDayDto>>> HandleAsync(
        GetResolvedEmployeeScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var resolvedList = await resolverService.ResolveRangeAsync(
            request.EmployeeId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        return resolvedList;
    }
}
