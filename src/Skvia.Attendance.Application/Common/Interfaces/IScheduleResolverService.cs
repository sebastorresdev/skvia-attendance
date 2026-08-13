using Skvia.Attendance.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IScheduleResolverService
{
    Task<ResolvedScheduleDayDto?> ResolveForDayAsync(
        Guid employeeId,
        DateOnly date,
        CancellationToken cancellationToken = default);

    Task<List<ResolvedScheduleDayDto>> ResolveRangeAsync(
        Guid employeeId,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, List<ResolvedScheduleDayDto>>> ResolveGridAsync(
        List<Guid> employeeIds,
        DateOnly startDate,
        DateOnly endDate,
        CancellationToken cancellationToken = default);
}
