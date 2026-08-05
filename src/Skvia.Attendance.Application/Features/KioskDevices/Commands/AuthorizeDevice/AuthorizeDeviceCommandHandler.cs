using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Branches;
using System.Security.Cryptography;

namespace Skvia.Attendance.Application.Features.KioskDevices.Commands.AuthorizeDevice;

public class AuthorizeDeviceCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<AuthorizeDeviceCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> HandleAsync(AuthorizeDeviceCommand command, CancellationToken cancellationToken)
    {
        var branch = await dbContext.Branches.FindAsync(new object[] { command.BranchId }, cancellationToken);
        if (branch is null)
            return BranchErrors.NotFound;

        // Generate a secure random token
        var tokenBytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(tokenBytes);
        }
        var token = Convert.ToBase64String(tokenBytes);

        var device = KioskDevice.Create(command.Name, command.BranchId, token);
        
        dbContext.KioskDevices.Add(device);
        await dbContext.SaveChangesAsync(cancellationToken);

        return token; // Return token to be sent to the Kiosk
    }
}
