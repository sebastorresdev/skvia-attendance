using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Branches;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.CheckOut;

public class CheckOutCommandHandler(
    IApplicationDbContext dbContext,
    IClock clock,
    ITimeZoneProvider timeZoneProvider) : ICommandHandler<CheckOutCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(CheckOutCommand command, CancellationToken cancellationToken)
    {
        // 1. Find Employee
        var identifier = command.EmployeeIdentifier.Trim().ToUpper();
        var employee = await dbContext.Employees
            .Include(e => e.EmployeeSchedules)
            .FirstOrDefaultAsync(e => 
                (e.Code == identifier || e.DocumentIdentifier.Number == identifier) 
                && e.Status == EmployeeStatus.Active, 
                cancellationToken);

        if (employee is null)
            return Error.NotFound("Employee.NotFound", "No se encontró un empleado activo con ese código o DNI.");

        // 2. Determine Date 
        var branch = await dbContext.Branches.FindAsync(new object[] { command.BranchId }, cancellationToken);
        if (branch is null)
            return BranchErrors.NotFound;

        var localTime = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZoneProvider.GetTimeZone(branch.TimeZoneId));
        var currentDate = DateOnly.FromDateTime(localTime.DateTime);

        // 3. Find Schedule to get total minutes scheduled
        var schedule = employee.EmployeeSchedules.FirstOrDefault(s => s.Date == currentDate);
        
        if (schedule is null || !schedule.AssignedStartTime.HasValue || !schedule.AssignedEndTime.HasValue)
            return Error.Validation("Schedule.Invalid", "No tienes horario completo asignado para el día de hoy.");

        var scheduledMinutes = (int)(schedule.AssignedEndTime.Value - schedule.AssignedStartTime.Value).TotalMinutes;

        // 4. Find today's Attendance (CheckIn) without CheckOut
        var attendance = await dbContext.Attendances
            .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.Date == currentDate && !a.CheckOut.HasValue, cancellationToken);

        if (attendance is null)
            return Error.Validation("Attendance.NotFound", "No has registrado tu entrada el día de hoy o ya registraste salida.");

        // 5. Register CheckOut
        attendance.RegisterCheckOut(command.BranchId, command.PhotoUrl, true, scheduledMinutes, clock);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
