using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Branches;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.EndBreak;

public class EndBreakCommandHandler(
    IApplicationDbContext dbContext,
    IClock clock,
    ITimeZoneProvider timeZoneProvider) : ICommandHandler<EndBreakCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(EndBreakCommand command, CancellationToken cancellationToken)
    {
        var identifier = command.EmployeeIdentifier.Trim().ToUpper();
        var employee = await dbContext.Employees
            .Include(e => e.EmployeeSchedules)
            .FirstOrDefaultAsync(e => 
                (e.Code == identifier || e.DocumentIdentifier.Number == identifier) 
                && e.Status == EmployeeStatus.Active, 
                cancellationToken);

        if (employee is null)
            return Error.NotFound("Employee.NotFound", "No se encontró un empleado activo con ese código o DNI.");

        var branch = await dbContext.Branches.FindAsync(new object[] { command.BranchId }, cancellationToken);
        if (branch is null)
            return BranchErrors.NotFound;

        bool isFourPointRequired = employee.RequireFourPointAttendance;
        if (!isFourPointRequired)
            return Error.Validation("Policy.Invalid", "Tu configuración de asistencia no requiere registro de refrigerio (2 puntos).");

        if (command.Source == AttendanceSource.Mobile)
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

        var localTime = TimeZoneInfo.ConvertTime(clock.UtcNow, timeZoneProvider.GetTimeZone(branch.TimeZoneId));
        var currentDate = DateOnly.FromDateTime(localTime.DateTime);

        var attendance = await dbContext.Attendances
            .FirstOrDefaultAsync(a => a.EmployeeId == employee.Id && a.Date == currentDate && !a.CheckOut.HasValue, cancellationToken);

        if (attendance is null)
            return Error.Validation("Attendance.NotFound", "No has registrado tu entrada el día de hoy o ya registraste salida.");

        if (!attendance.BreakStart.HasValue)
            return Error.Validation("Attendance.BreakNotStarted", "No has iniciado tu refrigerio.");

        if (attendance.BreakEnd.HasValue)
            return Error.Validation("Attendance.BreakAlreadyEnded", "Ya finalizaste tu refrigerio.");

        attendance.EndBreak(command.PhotoUrl, clock);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
