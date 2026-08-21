using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Workplaces;

namespace Skvia.Erp.Application.Features.Workplaces.Commands.CreateWorkplace;

public class CreateWorkplaceCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<CreateWorkplaceCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateWorkplaceCommand command, CancellationToken cancellationToken)
    {
        var existingCode = await dbContext.Workplaces
            .AnyAsync(w => w.Code.ToLower() == command.Code.Trim().ToLower(), cancellationToken);

        if (existingCode)
        {
            return Error.Conflict("Workplace.DuplicateCode", "Ya existe un lugar de marcación con ese código.");
        }

        var workplace = Workplace.Create(
            command.Code,
            command.Name,
            command.TimeZoneId,
            command.Latitude,
            command.Longitude,
            command.GeofenceRadiusMeters,
            command.Address,
            command.RequirePhotoForMobile);

        dbContext.Workplaces.Add(workplace);
        await dbContext.SaveChangesAsync(cancellationToken);

        return workplace.Id;
    }
}


