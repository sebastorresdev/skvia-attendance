using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Branches;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.CheckIn;

public class CheckInCommandHandler(
    IApplicationDbContext dbContext,
    IClock clock,
    ITimeZoneProvider timeZoneProvider) : ICommandHandler<CheckInCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(CheckInCommand command, CancellationToken cancellationToken)
    {
        // 1. Find Employee by Code or DNI
        var identifier = command.EmployeeIdentifier.Trim().ToUpper();
        var employee = await dbContext.Employees
            .Include(e => e.EmployeeSchedules)
            .FirstOrDefaultAsync(e => 
                (e.Code == identifier || e.DocumentIdentifier.Number == identifier) 
                && e.Status == EmployeeStatus.Active, 
                cancellationToken);

        if (employee is null)
            return Error.NotFound("Employee.NotFound", "No se encontró un empleado activo con ese código o DNI.");

        // 2. Determine Date (using Branch TimeZone)
        // For MVP we assume a default timezone if branch not found, but we should find branch.
        var branch = await dbContext.Branches.FindAsync(new object[] { command.BranchId }, cancellationToken);
        if (branch is null)
            return BranchErrors.NotFound;

        var localTime = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZoneProvider.GetTimeZone(branch.TimeZoneId));
        var currentDate = DateOnly.FromDateTime(localTime.DateTime);

        // 3. Find Schedule for today
        var schedule = employee.EmployeeSchedules.FirstOrDefault(s => s.Date == currentDate);
        
        if (schedule is null)
            return Error.Validation("Schedule.NotFound", "No tienes horario asignado para el día de hoy.");

        if (schedule.DayType != Skvia.Attendance.Domain.EmployeeSchedules.ScheduleDayType.WorkDay)
            return Error.Validation("Schedule.NotWorkday", "Hoy no es un día laborable según tu horario.");

        if (!schedule.AssignedStartTime.HasValue)
            return Error.Validation("Schedule.Invalid", "Tu horario de hoy no tiene hora de entrada configurada.");

        // 4. Create Attendance
        // For MVP, isValidCheckIn is true, we could add geo-validation later.
        var attendance = Skvia.Attendance.Domain.Attendances.Attendance.CreateCheckIn(
            employee.Id,
            command.BranchId,
            command.PhotoUrl,
            true, // isValidCheckIn
            schedule.AssignedStartTime.Value,
            branch.TimeZoneId,
            clock,
            timeZoneProvider);

        dbContext.Attendances.Add(attendance);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
