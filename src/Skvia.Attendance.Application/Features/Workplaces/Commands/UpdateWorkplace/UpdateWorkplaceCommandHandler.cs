using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Workplaces.Commands.UpdateWorkplace;

public class UpdateWorkplaceCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<UpdateWorkplaceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(UpdateWorkplaceCommand command, CancellationToken cancellationToken)
    {
        var workplace = await dbContext.Workplaces.FindAsync(new object[] { command.Id }, cancellationToken);
        if (workplace is null)
        {
            return Error.NotFound("Workplace.NotFound", "Lugar de marcación no encontrado.");
        }

        var existingCode = await dbContext.Workplaces
            .AnyAsync(w => w.Id != command.Id && w.Code.ToLower() == command.Code.Trim().ToLower(), cancellationToken);

        if (existingCode)
        {
            return Error.Conflict("Workplace.DuplicateCode", "Ya existe otro lugar de marcación con ese código.");
        }

        workplace.Update(
            command.Code,
            command.Name,
            command.TimeZoneId,
            command.Latitude,
            command.Longitude,
            command.GeofenceRadiusMeters,
            command.Address,
            command.RequirePhotoForMobile);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}
