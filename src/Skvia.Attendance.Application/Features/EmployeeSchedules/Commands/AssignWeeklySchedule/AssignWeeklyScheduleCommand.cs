using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignWeeklySchedule;

public record DailyScheduleRequest(
    DateOnly Date,
    Guid BranchId,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    ScheduleDayType DayType,
    Guid? BaseScheduleId = null);

public record AssignWeeklyScheduleCommand(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate,
    List<DailyScheduleRequest> Days) : ICommand<ErrorOr<Success>>;
