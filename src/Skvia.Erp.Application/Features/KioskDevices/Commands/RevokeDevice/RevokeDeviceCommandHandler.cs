using Skvia.Erp.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Kiosks;

namespace Skvia.Erp.Application.Features.KioskDevices.Commands.RevokeDevice;

public class RevokeDeviceCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<RevokeDeviceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(RevokeDeviceCommand command, CancellationToken cancellationToken)
    {
        var device = await dbContext.KioskDevices.FindAsync(new object[] { command.DeviceId }, cancellationToken);
        
        if (device is null)
            return Error.NotFound("KioskDevice.NotFound", "Dispositivo no encontrado.");

        if (device.Status != KioskDeviceStatus.Linked)
        {
            return Error.Validation("KioskDevice.InvalidState", "Solo se pueden inhabilitar kioskos que se encuentren actualmente Vinculados.");
        }

        device.Disable();
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


