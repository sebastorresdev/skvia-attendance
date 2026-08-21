using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;

using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.KioskDevices.Commands.AuthorizeDevice;

public record AuthorizeDeviceResult(
    Guid DeviceId,
    string Name,
    Guid WorkplaceId,
    string WorkplaceName,
    string Token,
    string PairingCode,
    DateTime ExpiresAt);

[AuthorizeCommand(Permissions = Permission.KioskDevices.Link)]
public record AuthorizeDeviceCommand(string Name, Guid WorkplaceId) : ICommand<ErrorOr<AuthorizeDeviceResult>>;



