using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Schedules;

namespace Skvia.Attendance.Application.Features.Schedules.Commands.CreateSchedule;

public class CreateScheduleCommandHandler(IScheduleRepository scheduleRepository, IApplicationDbContext dbContext) : ICommandHandler<CreateScheduleCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateScheduleCommand command, CancellationToken cancellationToken)
    {
        var schedule = Schedule.Create(command.Name, command.DefaultStartTime, command.DefaultEndTime);

        await scheduleRepository.AddAsync(schedule, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return schedule.Id;
    }
}
