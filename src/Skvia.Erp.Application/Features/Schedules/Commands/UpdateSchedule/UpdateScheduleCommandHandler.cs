using Skvia.Erp.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Schedules;

namespace Skvia.Erp.Application.Features.Schedules.Commands.UpdateSchedule;

public class UpdateScheduleCommandHandler(IScheduleRepository scheduleRepository, IApplicationDbContext dbContext) : ICommandHandler<UpdateScheduleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateScheduleCommand command, CancellationToken cancellationToken)
    {
        var schedule = await scheduleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (schedule is null)
            return Error.NotFound(description: "El turno no existe.");

        var updateResult = schedule.UpdateTimes(
            command.Code, 
            command.Description,
            command.TimeZoneId,
            command.DefaultStartTime, 
            command.DefaultEndTime,
            command.HasBreak,
            command.BreakStartTime,
            command.BreakEndTime);

        if (updateResult.IsError)
            return updateResult.Errors;

        scheduleRepository.Update(schedule);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


