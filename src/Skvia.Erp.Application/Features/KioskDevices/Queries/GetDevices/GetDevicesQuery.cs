using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.KioskDevices.Queries.GetDevices;

public record KioskDeviceDto(
    Guid Id,
    string Name,
    Guid WorkplaceId,
    string WorkplaceName,
    int Status,
    bool IsActive,
    string? PairingCode,
    DateTime? PairingCodeExpiresAt,
    DateTime? LinkedAt,
    DateTimeOffset CreatedAt);

[AuthorizeCommand(Permissions = Permission.KioskDevices.View)]
public record GetDevicesQuery() : IQuery<ErrorOr<IReadOnlyList<KioskDeviceDto>>>;


