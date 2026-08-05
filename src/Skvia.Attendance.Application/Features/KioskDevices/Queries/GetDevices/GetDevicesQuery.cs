using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

using Skvia.Attendance.Application.Common.Security;

namespace Skvia.Attendance.Application.Features.KioskDevices.Queries.GetDevices;

public record KioskDeviceDto(
    Guid Id,
    string Name,
    Guid BranchId,
    string BranchName,
    bool IsActive,
    DateTimeOffset CreatedAt);

[AuthorizeCommand(Permissions = Permission.KioskDevices.View)]
public record GetDevicesQuery() : IQuery<ErrorOr<IReadOnlyList<KioskDeviceDto>>>;
