namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.DeleteScheduleException;

public class DeleteScheduleExceptionCommandHandler(IApplicationDbContext dbContext)
    : ICommandHandler<DeleteScheduleExceptionCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteScheduleExceptionCommand request, CancellationToken cancellationToken)
    {
        var exception = await dbContext.ScheduleExceptions
            .FirstOrDefaultAsync(se => se.Id == request.Id, cancellationToken);

        if (exception is null)
            return Error.NotFound("ScheduleException.NotFound", "La excepción especificada no existe.");

        dbContext.ScheduleExceptions.Remove(exception);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
