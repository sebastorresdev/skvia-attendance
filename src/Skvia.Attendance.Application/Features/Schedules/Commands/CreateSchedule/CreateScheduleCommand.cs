using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Schedules.Commands.CreateSchedule;

public record CreateScheduleCommand(
    string Code,
    string Description,
    string TimeZoneId,
    TimeOnly DefaultStartTime,
    TimeOnly DefaultEndTime,
    bool HasBreak = false,
    TimeOnly? BreakStartTime = null,
    TimeOnly? BreakEndTime = null) : ICommand<ErrorOr<Guid>>;
