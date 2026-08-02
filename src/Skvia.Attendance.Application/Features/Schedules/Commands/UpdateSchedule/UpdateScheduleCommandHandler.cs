using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Schedules;

namespace Skvia.Attendance.Application.Features.Schedules.Commands.UpdateSchedule;

public class UpdateScheduleCommandHandler(IScheduleRepository scheduleRepository, IApplicationDbContext dbContext) : ICommandHandler<UpdateScheduleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateScheduleCommand command, CancellationToken cancellationToken)
    {
        var schedule = await scheduleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (schedule is null)
            return Error.NotFound(description: "El turno no existe.");

        schedule.UpdateTimes(command.Name, command.DefaultStartTime, command.DefaultEndTime);

        scheduleRepository.Update(schedule);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
