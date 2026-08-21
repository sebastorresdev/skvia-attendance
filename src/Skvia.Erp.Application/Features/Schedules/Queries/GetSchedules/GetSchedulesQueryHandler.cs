using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Schedules.DTOs;
using Skvia.Erp.Domain.Schedules;

namespace Skvia.Erp.Application.Features.Schedules.Queries.GetSchedules;

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


