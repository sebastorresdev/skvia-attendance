using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.CreateScheduleException;

public record CreateScheduleExceptionCommand(
    Guid EmployeeId,
    DateOnly Date,
    ScheduleDayType DayType,
    Guid? CustomScheduleId,
    bool IsDayOff,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Reason) : ICommand<ErrorOr<Guid>>;
