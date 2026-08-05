using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

using Skvia.Attendance.Application.Common.Security;

namespace Skvia.Attendance.Application.Features.KioskDevices.Commands.AuthorizeDevice;

[AuthorizeCommand(Permissions = Permission.KioskDevices.Link)]
public record AuthorizeDeviceCommand(string Name, Guid BranchId) : ICommand<ErrorOr<string>>;
