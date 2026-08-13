using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.RecalculateAttendance;

public class RecalculateAttendanceCommandHandler(
    IApplicationDbContext dbContext,
    IScheduleResolverService scheduleResolver,
    ITimeZoneProvider timeZoneProvider) : ICommandHandler<RecalculateAttendanceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(RecalculateAttendanceCommand command, CancellationToken cancellationToken)
    {
        var attendance = await dbContext.Attendances
            .Include(a => a.CheckInWorkplace)
            .FirstOrDefaultAsync(a => a.Id == command.AttendanceId, cancellationToken);

        if (attendance is null)
            return Error.NotFound(description: "El registro de asistencia no fue encontrado.");

        var employee = await dbContext.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == attendance.EmployeeId, cancellationToken);

        if (employee is null)
            return Error.NotFound(description: "Empleado no encontrado.");

        var resolvedSchedule = await scheduleResolver.ResolveForDayAsync(attendance.EmployeeId, attendance.Date, cancellationToken);

        if (resolvedSchedule is null || (resolvedSchedule.DayType != ScheduleDayType.WorkDay && resolvedSchedule.DayType != ScheduleDayType.MakeUpDay) || !resolvedSchedule.StartTime.HasValue)
        {
            attendance.RecalculateNonWorkDay();
            await dbContext.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }

        int totalScheduledMinutes = 0;
        if (resolvedSchedule.EndTime.HasValue)
        {
            totalScheduledMinutes = (int)(resolvedSchedule.EndTime.Value - resolvedSchedule.StartTime.Value).TotalMinutes;
            if (totalScheduledMinutes < 0) totalScheduledMinutes += 24 * 60; // Cross-midnight
        }

        var timeZoneId = attendance.CheckInWorkplace?.TimeZoneId ?? "America/Lima";

        attendance.Recalculate(
            resolvedSchedule.StartTime.Value,
            timeZoneId,
            timeZoneProvider,
            employee.TardinessToleranceMinutes,
            totalScheduledMinutes);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
