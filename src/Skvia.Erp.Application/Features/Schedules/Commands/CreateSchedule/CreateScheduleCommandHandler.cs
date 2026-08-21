using Skvia.Erp.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Schedules;

namespace Skvia.Erp.Application.Features.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandHandler(IScheduleRepository scheduleRepository, IApplicationDbContext dbContext) : ICommandHandler<CreateScheduleCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateScheduleCommand command, CancellationToken cancellationToken)
    {
        var scheduleResult = Schedule.Create(
            command.Code, 
            command.Description,
            command.TimeZoneId,
            command.DefaultStartTime, 
            command.DefaultEndTime,
            command.HasBreak,
            command.BreakStartTime,
            command.BreakEndTime);

        if (scheduleResult.IsError)
            return scheduleResult.Errors;

        await scheduleRepository.AddAsync(scheduleResult.Value, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return scheduleResult.Value.Id;
    }
}


