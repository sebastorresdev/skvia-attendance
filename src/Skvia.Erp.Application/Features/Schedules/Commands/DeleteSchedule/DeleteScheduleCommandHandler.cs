using Skvia.Erp.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Schedules;

namespace Skvia.Erp.Application.Features.Schedules.Commands.DeleteSchedule;

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


