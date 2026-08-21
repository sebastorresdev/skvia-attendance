using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Queries.GetEmployeeSchedule;

public record GetEmployeeScheduleQuery(
    Guid EmployeeId,
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<ErrorOr<List<EmployeeScheduleResponse>>>;


