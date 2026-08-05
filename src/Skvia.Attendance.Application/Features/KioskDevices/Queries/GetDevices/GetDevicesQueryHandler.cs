using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.KioskDevices.Queries.GetDevices;

public class GetDevicesQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetDevicesQuery, ErrorOr<IReadOnlyList<KioskDeviceDto>>>
{
    public async Task<ErrorOr<IReadOnlyList<KioskDeviceDto>>> HandleAsync(GetDevicesQuery query, CancellationToken cancellationToken)
    {
        var devices = await dbContext.KioskDevices
            .Include(d => d.Branch)
            .OrderByDescending(d => d.Created)
            .Select(d => new KioskDeviceDto(
                d.Id,
                d.Name,
                d.BranchId,
                d.Branch.Name,
                d.IsActive,
                d.Created))
            .ToListAsync(cancellationToken);

        return devices;
    }
}
