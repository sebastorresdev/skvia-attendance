using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Schedules.DTOs;
using Skvia.Attendance.Domain.Schedules;

namespace Skvia.Attendance.Application.Features.Schedules.Queries.GetSchedules;

public class GetSchedulesQueryHandler(IScheduleRepository scheduleRepository) : IQueryHandler<GetSchedulesQuery, ErrorOr<List<ScheduleResponse>>>
{
    public async Task<ErrorOr<List<ScheduleResponse>>> HandleAsync(GetSchedulesQuery query, CancellationToken cancellationToken)
    {
        var schedules = await scheduleRepository.GetAllAsync(cancellationToken);

        return schedules.Select(s => new ScheduleResponse(
            s.Id,
            s.Code,
            s.Description,
            s.TimeZoneId,
            s.HasBreak,
            s.BreakStartTime,
            s.BreakEndTime,
            s.DefaultStartTime,
            s.DefaultEndTime
        )).ToList();
    }
}
