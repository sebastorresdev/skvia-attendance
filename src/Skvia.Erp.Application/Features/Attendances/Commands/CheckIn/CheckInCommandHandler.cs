using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Attendances;
using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.Employees;
using Skvia.Erp.Domain.Workplaces;
using Skvia.Erp.Domain.Kiosks;
using ErrorOr;

using Microsoft.EntityFrameworkCore;

namespace Skvia.Erp.Application.Features.Attendances.Commands.CheckIn;

public class CheckInCommandHandler(
    IApplicationDbContext dbContext,
    IClock clock,
    ITimeZoneProvider timeZoneProvider,
    IScheduleResolverService scheduleResolver) : ICommandHandler<CheckInCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(CheckInCommand command, CancellationToken cancellationToken)
    {
        // 1. Find Employee by Code or DNI
        var identifier = command.EmployeeIdentifier.Trim().ToUpper();
        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(e => 
                (e.Code == identifier || e.DocumentIdentifier.Number == identifier) 
                && e.Status == EmployeeStatus.Active, cancellationToken);

        if (employee is null)
            return Error.NotFound("Employee.NotFound", "No se encontró un empleado activo con ese código o DNI.");

        // 1.5 Get Workplace (Needed for timezone, geofencing, and policies)
        var workplace = await dbContext.Workplaces.FindAsync(new object[] { command.WorkplaceId }, cancellationToken);
        if (workplace is null)
            return Error.NotFound("Workplace.NotFound", "Sede o lugar de marcación no encontrado.");

        // 1.6 Validate Source, Security, and Policies
        if (command.Source == AttendanceSource.Kiosk)
        {
            if (string.IsNullOrWhiteSpace(command.Token))
                return Error.Unauthorized("Kiosk.Unauthorized", "Dispositivo no autorizado.");

            var device = await dbContext.KioskDevices
                .FirstOrDefaultAsync(d => d.Token == command.Token && d.Status == KioskDeviceStatus.Linked, cancellationToken);


            if (device is null)
                return Error.Unauthorized("Kiosk.Unauthorized", "Dispositivo revocado o no encontrado.");
                
            if (device.WorkplaceId != command.WorkplaceId)
                return Error.Unauthorized("Kiosk.InvalidWorkplace", "El dispositivo no está asignado a esta sede/lugar.");

            if (employee.AllowedWorkplaceIds.Count > 0 && !employee.AllowedWorkplaceIds.Contains(device.WorkplaceId))
                return Error.Forbidden("Kiosk.EmployeeNotAllowed", "No tienes permisos para marcar asistencia en este lugar.");
        }
        else if (command.Source == AttendanceSource.Mobile)
        {
            if (!employee.MobileCheckInEnabled)
                return Error.Forbidden("Mobile.Forbidden", "No tienes habilitada la marcación móvil. Consulta con RRHH.");
            
            if (workplace.RequirePhotoForMobile && string.IsNullOrWhiteSpace(command.PhotoUrl))
                return Error.Validation("Mobile.PhotoRequired", "La foto es obligatoria para marcación móvil.");

            if (workplace.Latitude.HasValue && workplace.Longitude.HasValue && workplace.GeofenceRadiusMeters > 0)
            {
                if (!command.Latitude.HasValue || !command.Longitude.HasValue)
                    return Error.Validation("Mobile.GpsRequired", "Se requiere tu ubicación GPS para marcar en este lugar.");

                var distance = Common.Utils.GeoUtils.CalculateDistanceMeters(
                    workplace.Latitude.Value, workplace.Longitude.Value,
                    command.Latitude.Value, command.Longitude.Value);

                if (distance > workplace.GeofenceRadiusMeters)
                    return Error.Validation("Mobile.GpsOutOfRange", $"Marcación fuera del rango GPS permitido del lugar (Distancia: {Math.Round(distance)}m, Máximo: {workplace.GeofenceRadiusMeters}m).");
            }
        }

        // 2. Determine Date (using Workplace TimeZone)
        var localTime = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZoneProvider.GetTimeZone(workplace.TimeZoneId));
        var currentDate = DateOnly.FromDateTime(localTime.DateTime);

        var hireDateOnly = DateOnly.FromDateTime(employee.HireDate.Date);
        if (currentDate < hireDateOnly)
            return Error.Validation("Employee.NotHiredYet", $"No se puede registrar asistencia antes de la fecha de ingreso del empleado ({hireDateOnly:dd/MM/yyyy}).");

        // 3. Find Resolved Schedule for today
        var schedule = await scheduleResolver.ResolveForDayAsync(employee.Id, currentDate, cancellationToken);
        
        if (schedule is null || schedule.DayType == Skvia.Erp.Domain.EmployeeSchedules.ScheduleDayType.DayOff)
            return Error.Validation("Schedule.NotWorkday", "Hoy no es un día laborable según tu horario.");

        if (schedule.DayType != Skvia.Erp.Domain.EmployeeSchedules.ScheduleDayType.WorkDay && schedule.DayType != Skvia.Erp.Domain.EmployeeSchedules.ScheduleDayType.MakeUpDay)
            return Error.Validation("Schedule.NotWorkday", "Hoy no es un día laborable según tu horario.");

        if (!schedule.StartTime.HasValue)
            return Error.Validation("Schedule.Invalid", "Tu horario de hoy no tiene hora de entrada configurada.");

        // 3.5 Check if already checked in today
        var alreadyCheckedIn = await dbContext.Attendances
            .AnyAsync(a => a.EmployeeId == employee.Id && a.Date == currentDate, cancellationToken);

        if (alreadyCheckedIn)
            return Error.Conflict("Attendance.AlreadyCheckedIn", "Ya has registrado tu asistencia para el día de hoy.");

        // 4. Create Attendance
        // For MVP, isValidCheckIn is true, we could add geo-validation later.
        var attendance = Skvia.Erp.Domain.Attendances.Attendance.CreateCheckIn(
            employee.Id,
            command.WorkplaceId,
            command.PhotoUrl,
            true, // isValidCheckIn
            schedule.StartTime.Value,
            workplace.TimeZoneId,
            clock,
            timeZoneProvider,
            command.Source,
            command.Latitude,
            command.Longitude,
            command.DeviceId,
            employee.TardinessToleranceMinutes);

        dbContext.Attendances.Add(attendance);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


