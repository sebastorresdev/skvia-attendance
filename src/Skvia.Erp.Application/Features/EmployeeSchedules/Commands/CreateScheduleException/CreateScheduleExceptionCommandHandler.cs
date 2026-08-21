using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Commands.CreateScheduleException;

public class CreateScheduleExceptionCommandHandler(
    IApplicationDbContext dbContext,
    IScheduleResolverService scheduleResolver,
    ITimeZoneProvider timeZoneProvider)
    : ICommandHandler<CreateScheduleExceptionCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateScheduleExceptionCommand request, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee is null)
            return Error.NotFound("Employee.NotFound", "Empleado no encontrado.");

        var existingException = await dbContext.ScheduleExceptions
            .FirstOrDefaultAsync(se => se.EmployeeId == request.EmployeeId && se.Date == request.Date, cancellationToken);

        Guid exceptionId;

        if (existingException is not null)
        {
            var updateRes = existingException.Update(
                request.DayType,
                request.CustomScheduleId,
                request.IsDayOff,
                request.StartTime,
                request.EndTime,
                request.Reason);

            if (updateRes.IsError)
                return updateRes.Errors;

            exceptionId = existingException.Id;
        }
        else
        {
            var createRes = ScheduleException.Create(
                request.EmployeeId,
                request.Date,
                request.DayType,
                request.CustomScheduleId,
                request.IsDayOff,
                request.StartTime,
                request.EndTime,
                request.Reason);

            if (createRes.IsError)
                return createRes.Errors;

            await dbContext.ScheduleExceptions.AddAsync(createRes.Value, cancellationToken);
            exceptionId = createRes.Value.Id;
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // Auto-recalculate existing attendance record for this day if it exists
        var existingAttendance = await dbContext.Attendances
            .Include(a => a.CheckInWorkplace)
            .FirstOrDefaultAsync(a => a.EmployeeId == request.EmployeeId && a.Date == request.Date, cancellationToken);

        if (existingAttendance is not null)
        {
            var resolvedSchedule = await scheduleResolver.ResolveForDayAsync(request.EmployeeId, request.Date, cancellationToken);

            if (resolvedSchedule is not null && (resolvedSchedule.DayType == ScheduleDayType.WorkDay || resolvedSchedule.DayType == ScheduleDayType.MakeUpDay) && resolvedSchedule.StartTime.HasValue)
            {
                int totalScheduledMinutes = 0;
                if (resolvedSchedule.EndTime.HasValue)
                {
                    totalScheduledMinutes = (int)(resolvedSchedule.EndTime.Value - resolvedSchedule.StartTime.Value).TotalMinutes;
                    if (totalScheduledMinutes < 0) totalScheduledMinutes += 24 * 60;
                }

                var timeZoneId = existingAttendance.CheckInWorkplace?.TimeZoneId ?? "America/Lima";

                existingAttendance.Recalculate(
                    resolvedSchedule.StartTime.Value,
                    timeZoneId,
                    timeZoneProvider,
                    employee.TardinessToleranceMinutes,
                    totalScheduledMinutes);

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        return exceptionId;
    }
}


