using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

using Skvia.Attendance.Application.Common.Security;

namespace Skvia.Attendance.Application.Features.KioskDevices.Commands.AuthorizeDevice;

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

