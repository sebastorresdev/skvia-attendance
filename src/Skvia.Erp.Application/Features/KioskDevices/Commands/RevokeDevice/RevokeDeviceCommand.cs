using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.KioskDevices.Commands.RevokeDevice;

[AuthorizeCommand(Permissions = Permission.KioskDevices.Revoke)]
public record RevokeDeviceCommand(Guid DeviceId) : ICommand<ErrorOr<Success>>;


