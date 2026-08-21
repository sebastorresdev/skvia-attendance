using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Workplaces.Queries.GetWorkplaces;

public class GetWorkplacesQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetWorkplacesQuery, ErrorOr<IReadOnlyList<WorkplaceDto>>>
{
    public async Task<ErrorOr<IReadOnlyList<WorkplaceDto>>> HandleAsync(GetWorkplacesQuery query, CancellationToken cancellationToken)
    {
        var workplaces = await dbContext.Workplaces
            .AsNoTracking()
            .OrderBy(w => w.Name)
            .Select(w => new WorkplaceDto(
                w.Id,
                w.Code,
                w.Name,
                w.Address,
                w.TimeZoneId,
                w.Latitude,
                w.Longitude,
                w.GeofenceRadiusMeters,
                w.RequirePhotoForMobile))
            .ToListAsync(cancellationToken);

        return workplaces;
    }
}


