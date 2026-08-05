using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Application.Features.KioskDevices.Commands.RevokeDevice;

public class RevokeDeviceCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<RevokeDeviceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(RevokeDeviceCommand command, CancellationToken cancellationToken)
    {
        var device = await dbContext.KioskDevices.FindAsync(new object[] { command.DeviceId }, cancellationToken);
        
        if (device is null)
            return Error.NotFound("KioskDevice.NotFound", "Dispositivo no encontrado.");

        device.Revoke();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
