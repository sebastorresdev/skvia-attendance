using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Commands.AssignScheduleMatrix;

public record ScheduleMatrixCellItem(
    Guid EmployeeId,
    DateOnly Date,
    ScheduleDayType DayType,
    Guid? CustomScheduleId,
    TimeOnly? StartTime,
    TimeOnly? EndTime,
    string? Reason);

public record AssignScheduleMatrixCommand(
    List<ScheduleMatrixCellItem> Cells) : ICommand<ErrorOr<Success>>;


