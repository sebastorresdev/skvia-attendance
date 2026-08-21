using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Queries.GetResolvedSchedulesGrid;

/// <summary>
/// Consulta para obtener la grilla de horarios resueltos.
/// </summary>
[AuthorizeCommand(Permissions = Permission.EmployeeSchedule.View)]
public record EmployeeScheduleGridRowDto(
    Guid EmployeeId,
    string EmployeeName,
    string EmployeeCode,
    string? DepartmentName,
    string? BranchName,
    List<ResolvedScheduleDayDto> Days);

/// <summary>
/// Consulta para obtener la grilla de horarios resueltos.
/// </summary>
[AuthorizeCommand(Permissions = Permission.EmployeeSchedule.View)]
public record GetResolvedSchedulesGridQuery(
    Guid? BranchId,
    Guid? DepartmentId,
    DateOnly StartDate,
    DateOnly EndDate) : IQuery<ErrorOr<List<EmployeeScheduleGridRowDto>>>;



