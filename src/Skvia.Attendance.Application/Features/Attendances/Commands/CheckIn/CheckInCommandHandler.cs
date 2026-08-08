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

        // 1.5 Get Branch (Needed for timezone, geofencing, and policies)
        var branch = await dbContext.Branches.FindAsync(new object[] { command.BranchId }, cancellationToken);
        if (branch is null)
            return BranchErrors.NotFound;

        // 1.6 Validate Source, Security, and Policies
        if (command.Source == AttendanceSource.Kiosk)
        {
            if (string.IsNullOrWhiteSpace(command.Token))
                return Error.Unauthorized("Kiosk.Unauthorized", "Dispositivo no autorizado.");

            var device = await dbContext.KioskDevices
                .FirstOrDefaultAsync(d => d.Token == command.Token && d.IsActive, cancellationToken);

            if (device is null)
                return Error.Unauthorized("Kiosk.Unauthorized", "Dispositivo revocado o no encontrado.");
                
            if (device.BranchId != command.BranchId)
                return Error.Unauthorized("Kiosk.InvalidBranch", "El dispositivo no está asignado a esta sede.");
        }
        else if (command.Source == AttendanceSource.Mobile)
        {
            if (!employee.MobileCheckInEnabled)
                return Error.Forbidden("Mobile.Forbidden", "No tienes habilitada la marcación móvil. Consulta con RRHH.");
            
            if (branch.RequirePhotoForMobile && string.IsNullOrWhiteSpace(command.PhotoUrl))
                return Error.Validation("Mobile.PhotoRequired", "La foto es obligatoria para marcación móvil.");

            if (branch.Latitude.HasValue && branch.Longitude.HasValue && branch.GeofenceRadiusMeters.HasValue)
            {
                if (!command.Latitude.HasValue || !command.Longitude.HasValue)
                    return Error.Validation("Mobile.GpsRequired", "Se requiere tu ubicación GPS para marcar en esta sede.");

                var distance = Common.Utils.GeoUtils.CalculateDistanceMeters(
                    branch.Latitude.Value, branch.Longitude.Value,
                    command.Latitude.Value, command.Longitude.Value);

                if (distance > branch.GeofenceRadiusMeters.Value)
                    return Error.Validation("Mobile.GpsOutOfRange", $"Marcación fuera del rango GPS permitido de la sede (Distancia: {Math.Round(distance)}m, Máximo: {branch.GeofenceRadiusMeters.Value}m).");
            }
        }

        // 2. Determine Date (using Branch TimeZone)
        var localTime = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZoneProvider.GetTimeZone(branch.TimeZoneId));
        var currentDate = DateOnly.FromDateTime(localTime.DateTime);

        var hireDateOnly = DateOnly.FromDateTime(employee.HireDate.Date);
        if (currentDate < hireDateOnly)
            return Error.Validation("Employee.NotHiredYet", $"No se puede registrar asistencia antes de la fecha de ingreso del empleado ({hireDateOnly:dd/MM/yyyy}).");

        // 3. Find Schedule for today
        var schedule = employee.EmployeeSchedules.FirstOrDefault(s => s.Date == currentDate);
        
        if (schedule is null)
            return Error.Validation("Schedule.NotFound", "No tienes horario asignado para el día de hoy.");

        if (schedule.DayType != Skvia.Attendance.Domain.EmployeeSchedules.ScheduleDayType.WorkDay)
            return Error.Validation("Schedule.NotWorkday", "Hoy no es un día laborable según tu horario.");

        if (!schedule.AssignedStartTime.HasValue)
            return Error.Validation("Schedule.Invalid", "Tu horario de hoy no tiene hora de entrada configurada.");

        // 3.5 Check if already checked in today
        var alreadyCheckedIn = await dbContext.Attendances
            .AnyAsync(a => a.EmployeeId == employee.Id && a.Date == currentDate, cancellationToken);

        if (alreadyCheckedIn)
            return Error.Conflict("Attendance.AlreadyCheckedIn", "Ya has registrado tu asistencia para el día de hoy.");

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
            timeZoneProvider,
            command.Source,
            command.Latitude,
            command.Longitude,
            command.DeviceId,
            branch.TardinessToleranceMinutes);

        dbContext.Attendances.Add(attendance);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
