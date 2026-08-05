using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.KioskDevices.Commands.DeleteDevice;

public record DeleteDeviceCommand(Guid Id) : ICommand<ErrorOr<Success>>;
