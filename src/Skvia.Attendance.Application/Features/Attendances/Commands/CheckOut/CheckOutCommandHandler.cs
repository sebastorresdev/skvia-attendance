using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Workplaces;
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

        // 2. Determine Date and validate Source/Policies
        var workplace = await dbContext.Workplaces.FindAsync(new object[] { command.WorkplaceId }, cancellationToken);
        if (workplace is null)
            return Error.NotFound("Workplace.NotFound", "Sede o lugar de marcación no encontrado.");

        if (command.Source == AttendanceSource.Kiosk)
        {
            if (string.IsNullOrWhiteSpace(command.Token))
                return Error.Unauthorized("Kiosk.Unauthorized", "Dispositivo no autorizado.");

            var device = await dbContext.KioskDevices
                .FirstOrDefaultAsync(d => d.Token == command.Token && d.IsActive, cancellationToken);

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

        var localTime = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZoneProvider.GetTimeZone(workplace.TimeZoneId));
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
        attendance.RegisterCheckOut(command.WorkplaceId, command.PhotoUrl, true, scheduledMinutes, clock);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
