using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Application.Features.Schedules.Commands.CreateSchedule;

public record CreateScheduleCommand(
    string Name,
    TimeOnly DefaultStartTime,
    TimeOnly DefaultEndTime) : ICommand<ErrorOr<Guid>>;
