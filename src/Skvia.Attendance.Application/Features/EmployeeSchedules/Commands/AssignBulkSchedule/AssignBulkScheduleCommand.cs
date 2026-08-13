using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Application.Features.EmployeeSchedules.Commands.AssignBulkSchedule;

public record AssignBulkScheduleCommand(
    Guid ScheduleTemplateId,
    List<Guid> EmployeeIds,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo) : ICommand<ErrorOr<Success>>;
