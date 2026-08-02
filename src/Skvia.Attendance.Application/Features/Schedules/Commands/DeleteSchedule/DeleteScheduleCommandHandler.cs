using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Schedules;

namespace Skvia.Attendance.Application.Features.Schedules.Commands.DeleteSchedule;

public class DeleteScheduleCommandHandler(IScheduleRepository scheduleRepository, IApplicationDbContext dbContext) : ICommandHandler<DeleteScheduleCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteScheduleCommand command, CancellationToken cancellationToken)
    {
        var schedule = await scheduleRepository.GetByIdAsync(command.Id, cancellationToken);

        if (schedule is null)
            return Error.NotFound(description: "El turno no existe.");

        scheduleRepository.Remove(schedule);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
