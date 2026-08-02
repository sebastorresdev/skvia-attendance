namespace Skvia.Attendance.Application.Features.Schedules.DTOs;

public record ScheduleResponse(
    Guid Id,
    string Name,
    TimeOnly DefaultStartTime,
    TimeOnly DefaultEndTime);
