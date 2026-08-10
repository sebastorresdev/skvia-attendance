using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Security;

namespace Skvia.Attendance.Application.Features.Workplaces.Commands.DeleteWorkplace;

[AuthorizeCommand(Permissions = Permission.Workplace.Delete)]
public record DeleteWorkplaceCommand(Guid Id) : ICommand<ErrorOr<Success>>;
