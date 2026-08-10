using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.GenerateSchedules;

public class GenerateSchedulesCommandHandler(IApplicationDbContext dbContext) : ICommandHandler<GenerateSchedulesCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(GenerateSchedulesCommand command, CancellationToken cancellationToken)
    {
        if (command.StartDate > command.EndDate)
        {
            return Error.Validation("Schedules.InvalidDateRange", "La fecha de inicio debe ser menor o igual a la fecha de fin.");
        }

        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == command.EmployeeId, cancellationToken);

        if (employee is null)
        {
            return EmployeeErrors.NotFound;
        }

        if (employee.MainBranchId is null)
        {
            return Error.Validation("Schedules.NoMainBranch", "El empleado debe tener una sede principal asignada para generarle horarios.");
        }

        if (command.Patterns == null || command.Patterns.Count == 0)
        {
            return Error.Validation("Schedules.NoPattern", "Debe enviar un patrón de horario configurado.");
        }

        if (employee.RequireFourPointAttendance && command.Patterns != null)
        {
            var baseScheduleIds = command.Patterns
                .Where(p => p.IsWorkDay && p.BaseScheduleId.HasValue)
                .Select(p => p.BaseScheduleId!.Value)
                .Distinct()
                .ToList();

            if (baseScheduleIds.Count > 0)
            {
                var invalidSchedules = await dbContext.Schedules
                    .Where(s => baseScheduleIds.Contains(s.Id) && !s.HasBreak)
                    .Select(s => s.Code)
                    .ToListAsync(cancellationToken);

                if (invalidSchedules.Count > 0)
                {
                    return Error.Validation(
                        "Schedules.FourPointMismatch",
                        $"El empleado requiere 4 marcaciones obligatorias (con refrigerio), pero los horarios asignados ({string.Join(", ", invalidSchedules)}) no contempla refrigerio (solo 2 marcaciones).");
                }
            }
        }

        var patternsDict = command.Patterns.ToDictionary(p => p.DayOfWeek);

        // Get existing schedules for the period
        var existingSchedules = await dbContext.EmployeeSchedules
            .Where(s => s.EmployeeId == employee.Id && s.Date >= command.StartDate && s.Date <= command.EndDate)
            .ToListAsync(cancellationToken);

        if (command.Patterns != null && command.Patterns.Count > 0 && existingSchedules.Count > 0)
        {
            // If patterns were explicitly passed, delete old range schedules to apply the new pattern
            dbContext.EmployeeSchedules.RemoveRange(existingSchedules);
            existingSchedules.Clear();
        }

        var existingDates = existingSchedules.Select(s => s.Date).ToHashSet();
        var branchId = employee.MainBranchId.Value;

        var newSchedules = new List<EmployeeSchedule>();

        for (var date = command.StartDate; date <= command.EndDate; date = date.AddDays(1))
        {
            if (existingDates.Contains(date))
                continue;

            if (!patternsDict.TryGetValue(date.DayOfWeek, out var pattern))
            {
                var restDayResult = EmployeeSchedule.CreateRestDay(employee.Id, date);
                if (!restDayResult.IsError) newSchedules.Add(restDayResult.Value);
                continue;
            }

            if (pattern.IsWorkDay && pattern.StartTime.HasValue && pattern.EndTime.HasValue)
            {
                var workDayResult = EmployeeSchedule.CreateWorkDay(employee.Id, date, pattern.StartTime.Value, pattern.EndTime.Value, pattern.BaseScheduleId);
                if (!workDayResult.IsError) newSchedules.Add(workDayResult.Value);
            }
            else
            {
                var restDayResult = EmployeeSchedule.CreateRestDay(employee.Id, date);
                if (!restDayResult.IsError) newSchedules.Add(restDayResult.Value);
            }
        }

        if (newSchedules.Count > 0)
        {
            dbContext.EmployeeSchedules.AddRange(newSchedules);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
