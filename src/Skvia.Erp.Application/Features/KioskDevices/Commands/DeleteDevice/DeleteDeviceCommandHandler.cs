using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.KioskDevices.Commands.DeleteDevice;

[AuthorizeCommand(Permissions = Permission.KioskDevices.Revoke)] // Reusing revoke permission for delete
public class DeleteDeviceCommandHandler(IApplicationDbContext dbContext) 
    : ICommandHandler<DeleteDeviceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteDeviceCommand command, CancellationToken cancellationToken)
    {
        var device = await dbContext.KioskDevices.FirstOrDefaultAsync(d => d.Id == command.Id, cancellationToken);
        
        if (device is null)
            return Error.NotFound("KioskDevice.NotFound", "El dispositivo no fue encontrado.");

        dbContext.KioskDevices.Remove(device);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


