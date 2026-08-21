using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.KioskDevices.Queries.GetDevices;

public class GetDevicesQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetDevicesQuery, ErrorOr<IReadOnlyList<KioskDeviceDto>>>
{
    public async Task<ErrorOr<IReadOnlyList<KioskDeviceDto>>> HandleAsync(GetDevicesQuery query, CancellationToken cancellationToken)
    {
        var devices = await dbContext.KioskDevices
            .Include(d => d.Workplace)
            .OrderByDescending(d => d.Created)
            .Select(d => new KioskDeviceDto(
                d.Id,
                d.Name,
                d.WorkplaceId,
                d.Workplace.Name,
                (int)d.Status,
                d.IsActive,
                d.PairingCode,
                d.PairingCodeExpiresAt,
                d.LinkedAt,
                d.Created))
            .ToListAsync(cancellationToken);

        return devices;
    }
}


