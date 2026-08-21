using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;
using ErrorOr;

namespace Skvia.Erp.Application.Features.KioskDevices.Commands.DeleteDevice;

public record DeleteDeviceCommand(Guid Id) : ICommand<ErrorOr<Success>>;


