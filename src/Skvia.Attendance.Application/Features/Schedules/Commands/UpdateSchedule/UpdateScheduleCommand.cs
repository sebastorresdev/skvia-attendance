using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Schedules.Commands.UpdateSchedule;

public record UpdateScheduleCommand(
    Guid Id,
    string Name,
    TimeOnly DefaultStartTime,
    TimeOnly DefaultEndTime) : ICommand<ErrorOr<Success>>;
