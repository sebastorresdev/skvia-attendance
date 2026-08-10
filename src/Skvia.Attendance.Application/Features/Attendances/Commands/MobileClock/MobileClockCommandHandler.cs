using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Attendances.Commands.CheckIn;
using Skvia.Attendance.Application.Features.Attendances.Commands.CheckOut;
using Skvia.Attendance.Application.Features.Attendances.Commands.EndBreak;
using Skvia.Attendance.Application.Features.Attendances.Commands.StartBreak;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Application.Common.Utils;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.MobileClock;

public class MobileClockCommandHandler(
    IApplicationDbContext dbContext,
    ICommandHandler<CheckInCommand, ErrorOr<Success>> checkInHandler,
    ICommandHandler<StartBreakCommand, ErrorOr<Success>> startBreakHandler,
    ICommandHandler<EndBreakCommand, ErrorOr<Success>> endBreakHandler,
    ICommandHandler<CheckOutCommand, ErrorOr<Success>> checkOutHandler) : ICommandHandler<MobileClockCommand, ErrorOr<MobileClockResult>>
{
    public async Task<ErrorOr<MobileClockResult>> HandleAsync(MobileClockCommand command, CancellationToken cancellationToken)
    {
        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(e => e.ApplicationUserId == command.ApplicationUserId || e.Code == command.UserName, cancellationToken);
            
        if (employee is null || !employee.AllowedWorkplaceIds.Any())
        {
            return Error.Validation("MobileClock.InvalidEmployee", "Empleado no encontrado o sin lugares de marcación asignados.");
        }

        if (!command.Latitud.HasValue || !command.Longitud.HasValue)
        {
            return Error.Validation("MobileClock.LocationRequired", "La ubicación (Latitud y Longitud) es obligatoria para marcaciones móviles.");
        }

        var workplaces = await dbContext.Workplaces
            .Where(w => employee.AllowedWorkplaceIds.Contains(w.Id))
            .ToListAsync(cancellationToken);

        Guid? validWorkplaceId = null;

        foreach (var workplace in workplaces)
        {
            if (!workplace.Latitude.HasValue || !workplace.Longitude.HasValue)
            {
                // Si el lugar no tiene ubicación configurada, se permite marcar sin restricción de geocerca
                validWorkplaceId = workplace.Id;
                break;
            }

            var distance = GeoUtils.CalculateDistanceMeters(
                command.Latitud.Value,
                command.Longitud.Value,
                workplace.Latitude.Value,
                workplace.Longitude.Value);

            if (distance <= workplace.GeofenceRadiusMeters)
            {
                validWorkplaceId = workplace.Id;
                break;
            }
        }

        if (validWorkplaceId is null)
        {
            return Error.Validation("MobileClock.OutOfRange", "Te encuentras fuera del área permitida para registrar tu asistencia en tus lugares asignados.");
        }

        var workplaceId = validWorkplaceId.Value;

        ErrorOr<Success> result;
        var photo = command.PhotoUrl ?? "mobile-default-photo.jpg"; // Default photo for testing

        switch (command.TipoMarcacion.ToUpper())
        {
            case "ENTRADA":
                result = await checkInHandler.HandleAsync(new CheckInCommand(employee.Code, workplaceId, photo, AttendanceSource.Mobile, command.Latitud, command.Longitud), cancellationToken);
                break;
            case "INICIO_REFRIGERIO":
                result = await startBreakHandler.HandleAsync(new StartBreakCommand(employee.Code, workplaceId, photo, AttendanceSource.Mobile, command.Latitud, command.Longitud), cancellationToken);
                break;
            case "FIN_REFRIGERIO":
                result = await endBreakHandler.HandleAsync(new EndBreakCommand(employee.Code, workplaceId, photo, AttendanceSource.Mobile, command.Latitud, command.Longitud), cancellationToken);
                break;
            case "SALIDA":
                result = await checkOutHandler.HandleAsync(new CheckOutCommand(employee.Code, workplaceId, photo, AttendanceSource.Mobile, command.Latitud, command.Longitud), cancellationToken);
                break;
            default:
                return Error.Validation("MobileClock.InvalidType", "Tipo de marcación inválido.");
        }

        if (result.IsError)
        {
            return result.Errors;
        }

        return new MobileClockResult(true, $"Marcación de {command.TipoMarcacion} registrada con éxito.", DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), command.TipoMarcacion, command.Latitud, command.Longitud);
    }
}
