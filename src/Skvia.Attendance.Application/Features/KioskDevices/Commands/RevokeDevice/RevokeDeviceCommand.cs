using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Application.Common.Security;

namespace Skvia.Attendance.Application.Features.KioskDevices.Commands.RevokeDevice;

[AuthorizeCommand(Permissions = Permission.KioskDevices.Revoke)]
public record RevokeDeviceCommand(Guid DeviceId) : ICommand<ErrorOr<Success>>;
