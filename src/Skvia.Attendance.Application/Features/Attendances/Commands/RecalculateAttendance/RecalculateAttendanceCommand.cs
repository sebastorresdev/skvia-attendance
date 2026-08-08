using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Application.Features.Attendances.Commands.RecalculateAttendance;

public record RecalculateAttendanceCommand(Guid AttendanceId) : ICommand<ErrorOr<Success>>;
