using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Workplaces;
using Skvia.Erp.Domain.Kiosks;
using System.Security.Cryptography;

namespace Skvia.Erp.Application.Features.KioskDevices.Commands.AuthorizeDevice;

public class AuthorizeDeviceCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<AuthorizeDeviceCommand, ErrorOr<AuthorizeDeviceResult>>
{
    public async Task<ErrorOr<AuthorizeDeviceResult>> HandleAsync(AuthorizeDeviceCommand command, CancellationToken cancellationToken)
    {
        var workplace = await dbContext.Workplaces.FindAsync(new object[] { command.WorkplaceId }, cancellationToken);
        if (workplace is null)
            return Error.NotFound("Workplace.NotFound", "Sede o lugar de marcación no encontrado.");

        // Generate a secure random token
        var tokenBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }
        var token = Convert.ToBase64String(tokenBytes);

        var device = KioskDevice.Create(command.Name, command.WorkplaceId, token);
        
        dbContext.KioskDevices.Add(device);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AuthorizeDeviceResult(
            device.Id,
            device.Name,
            device.WorkplaceId,
            workplace.Name,
            device.Token,
            string.Empty,
            DateTime.UtcNow);
    }
}


