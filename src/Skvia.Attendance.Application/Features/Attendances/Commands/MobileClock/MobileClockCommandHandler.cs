using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Features.Attendances.Commands.CheckIn;
using Skvia.Attendance.Application.Features.Attendances.Commands.CheckOut;
using Skvia.Attendance.Application.Features.Attendances.Commands.EndBreak;
using Skvia.Attendance.Application.Features.Attendances.Commands.StartBreak;
using Skvia.Attendance.Domain.Attendances;
using Skvia.Attendance.Domain.Common;

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
            
        if (employee is null || !employee.MainBranchId.HasValue)
        {
            return Error.Validation("MobileClock.InvalidEmployee", "Empleado no encontrado o sin sede principal asignada.");
        }

        ErrorOr<Success> result;
        var photo = command.PhotoUrl ?? "mobile-default-photo.jpg"; // Default photo for testing

        switch (command.TipoMarcacion.ToUpper())
        {
            case "ENTRADA":
                result = await checkInHandler.HandleAsync(new CheckInCommand(employee.Code, employee.MainBranchId.Value, photo, AttendanceSource.Mobile, command.Latitud, command.Longitud), cancellationToken);
                break;
            case "INICIO_REFRIGERIO":
                result = await startBreakHandler.HandleAsync(new StartBreakCommand(employee.Code, employee.MainBranchId.Value, photo, AttendanceSource.Mobile, command.Latitud, command.Longitud), cancellationToken);
                break;
            case "FIN_REFRIGERIO":
                result = await endBreakHandler.HandleAsync(new EndBreakCommand(employee.Code, employee.MainBranchId.Value, photo, AttendanceSource.Mobile, command.Latitud, command.Longitud), cancellationToken);
                break;
            case "SALIDA":
                result = await checkOutHandler.HandleAsync(new CheckOutCommand(employee.Code, employee.MainBranchId.Value, photo, AttendanceSource.Mobile, command.Latitud, command.Longitud), cancellationToken);
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
