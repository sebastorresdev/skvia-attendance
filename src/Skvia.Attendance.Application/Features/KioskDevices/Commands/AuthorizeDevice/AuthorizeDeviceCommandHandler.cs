using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Workplaces;
using Skvia.Attendance.Domain.Kiosks;
using System.Security.Cryptography;

namespace Skvia.Attendance.Application.Features.KioskDevices.Commands.AuthorizeDevice;

public class AuthorizeDeviceCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<AuthorizeDeviceCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> HandleAsync(AuthorizeDeviceCommand command, CancellationToken cancellationToken)
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

        return token; // Return token to be sent to the Kiosk
    }
}
