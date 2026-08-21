using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;

namespace Skvia.Erp.Application.Features.Attendances.Commands.RecalculateAttendance;

public record RecalculateAttendanceCommand(Guid AttendanceId) : ICommand<ErrorOr<Success>>;


