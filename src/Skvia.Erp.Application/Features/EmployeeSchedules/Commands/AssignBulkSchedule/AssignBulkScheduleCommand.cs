using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Commands.AssignBulkSchedule;

public record AssignBulkScheduleCommand(
    Guid ScheduleTemplateId,
    List<Guid> EmployeeIds,
    DateOnly EffectiveFrom,
    DateOnly? EffectiveTo) : ICommand<ErrorOr<Success>>;


