using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.DTOs;

public record EmployeeScheduleResponse(
    Guid Id,
    Guid EmployeeId,
    DateOnly Date,
    TimeOnly? AssignedStartTime,
    TimeOnly? AssignedEndTime,
    ScheduleDayType DayType,
    Guid? BaseScheduleId);

