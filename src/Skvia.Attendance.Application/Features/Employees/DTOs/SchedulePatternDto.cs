namespace Skvia.Attendance.Application.Features.Employees.DTOs;

public record SchedulePatternDto(
    DayOfWeek DayOfWeek,
    bool IsWorkDay,
    TimeOnly? StartTime,
    TimeOnly? EndTime
);
