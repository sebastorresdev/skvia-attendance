using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignScheduleMatrix;

public class AssignScheduleMatrixCommandHandler(
    IApplicationDbContext dbContext,
    IScheduleResolverService scheduleResolver,
    ITimeZoneProvider timeZoneProvider)
    : ICommandHandler<AssignScheduleMatrixCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(AssignScheduleMatrixCommand request, CancellationToken cancellationToken)
    {
        if (request.Cells.Count == 0)
            return Result.Success;

        var employeeIds = request.Cells.Select(c => c.EmployeeId).Distinct().ToList();
        var dates = request.Cells.Select(c => c.Date).Distinct().ToList();
        var minDate = dates.Min();
        var maxDate = dates.Max();

        // Buscar excepciones existentes en el rango
        var existingExceptions = await dbContext.ScheduleExceptions
            .Where(se => employeeIds.Contains(se.EmployeeId) && se.Date >= minDate && se.Date <= maxDate)
            .ToListAsync(cancellationToken);

        var existingDict = existingExceptions
            .ToDictionary(e => (e.EmployeeId, e.Date));

        var toAdd = new List<ScheduleException>();

        foreach (var cell in request.Cells)
        {
            if (existingDict.TryGetValue((cell.EmployeeId, cell.Date), out var existing))
            {
                var updateRes = existing.Update(
                    cell.DayType,
                    cell.CustomScheduleId,
                    cell.DayType == ScheduleDayType.DayOff,
                    cell.StartTime,
                    cell.EndTime,
                    cell.Reason);

                if (updateRes.IsError)
                    return updateRes.Errors;
            }
            else
            {
                var createRes = ScheduleException.Create(
                    cell.EmployeeId,
                    cell.Date,
                    cell.DayType,
                    cell.CustomScheduleId,
                    cell.DayType == ScheduleDayType.DayOff,
                    cell.StartTime,
                    cell.EndTime,
                    cell.Reason);

                if (createRes.IsError)
                    return createRes.Errors;

                toAdd.Add(createRes.Value);
            }
        }

        if (toAdd.Count > 0)
        {
            await dbContext.ScheduleExceptions.AddRangeAsync(toAdd, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        // Auto-recalculate any existing attendances in the updated date range
        var existingAttendances = await dbContext.Attendances
            .Include(a => a.CheckInWorkplace)
            .Include(a => a.Employee)
            .Where(a => employeeIds.Contains(a.EmployeeId) && a.Date >= minDate && a.Date <= maxDate)
            .ToListAsync(cancellationToken);

        if (existingAttendances.Count > 0)
        {
            foreach (var attendance in existingAttendances)
            {
                var resolvedSchedule = await scheduleResolver.ResolveForDayAsync(attendance.EmployeeId, attendance.Date, cancellationToken);
                if (resolvedSchedule is not null && (resolvedSchedule.DayType == ScheduleDayType.WorkDay || resolvedSchedule.DayType == ScheduleDayType.MakeUpDay) && resolvedSchedule.StartTime.HasValue)
                {
                    int totalScheduledMinutes = 0;
                    if (resolvedSchedule.EndTime.HasValue)
                    {
                        totalScheduledMinutes = (int)(resolvedSchedule.EndTime.Value - resolvedSchedule.StartTime.Value).TotalMinutes;
                        if (totalScheduledMinutes < 0) totalScheduledMinutes += 24 * 60;
                    }

                    var timeZoneId = attendance.CheckInWorkplace?.TimeZoneId ?? "America/Lima";

                    attendance.Recalculate(
                        resolvedSchedule.StartTime.Value,
                        timeZoneId,
                        timeZoneProvider,
                        attendance.Employee.TardinessToleranceMinutes,
                        totalScheduledMinutes);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return Result.Success;
    }
}
