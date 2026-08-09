using ErrorOr;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.GenerateSchedules;

public record GenerateSchedulesCommand(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate,
    List<SchedulePatternInput>? Patterns = null) : ICommand<ErrorOr<Success>>;

public record SchedulePatternInput(
    DayOfWeek DayOfWeek,
    bool IsWorkDay,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    Guid? BaseScheduleId = null);
