using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.EmployeeSchedules;
using System.Linq;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.RecalculateAttendance;

public class RecalculateAttendanceCommandHandler(
    IApplicationDbContext dbContext,
    ITimeZoneProvider timeZoneProvider) : ICommandHandler<RecalculateAttendanceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(RecalculateAttendanceCommand command, CancellationToken cancellationToken)
    {
        var attendance = await dbContext.Attendances
            .Include(a => a.CheckInWorkplace)
            .FirstOrDefaultAsync(a => a.Id == command.AttendanceId, cancellationToken);

        if (attendance is null)
            return Error.NotFound(description: "El registro de asistencia no fue encontrado.");

        // Obtenemos el horario (asumiendo que EmployeeSchedules está en dbContext. No está expuesto directamente a veces. Vamos a revisar cómo acceder a EmployeeSchedules)
        // Wait, I need to check if dbContext exposes EmployeeSchedules. Let's do a quick lookup via the Employee entity if not.
        var employee = await dbContext.Employees
            .Include(e => e.EmployeeSchedules)
            .FirstOrDefaultAsync(e => e.Id == attendance.EmployeeId, cancellationToken);
            
        if (employee is null)
            return Error.NotFound(description: "Empleado no encontrado.");

        var employeeSchedule = employee.EmployeeSchedules.FirstOrDefault(es => es.Date == attendance.Date);

        if (employeeSchedule is null)
            return Error.NotFound(description: "No se encontró programación de horario para este empleado en esta fecha.");

        if (employeeSchedule.DayType != ScheduleDayType.WorkDay && employeeSchedule.DayType != ScheduleDayType.MakeUpDay)
            return Error.Validation(description: "El día programado no es un día laborable y no puede recalcularse.");

        if (!employeeSchedule.AssignedStartTime.HasValue)
            return Error.Validation(description: "La programación no tiene una hora de entrada asignada.");

        int totalScheduledMinutes = 0;
        if (employeeSchedule.AssignedEndTime.HasValue)
        {
            totalScheduledMinutes = (int)(employeeSchedule.AssignedEndTime.Value - employeeSchedule.AssignedStartTime.Value).TotalMinutes;
            if (totalScheduledMinutes < 0) totalScheduledMinutes += 24 * 60; // Cross-midnight
        }

        attendance.Recalculate(
            employeeSchedule.AssignedStartTime.Value,
            attendance.CheckInWorkplace.TimeZoneId,
            timeZoneProvider,
            employee.TardinessToleranceMinutes, // Replaced branch tolerance with employee tolerance
            totalScheduledMinutes);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
