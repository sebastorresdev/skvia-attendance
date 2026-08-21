using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.EmployeeSchedules.DTOs;

namespace Skvia.Erp.Application.Features.EmployeeSchedules.Queries.GetResolvedEmployeeSchedule;

public class GetResolvedEmployeeScheduleQueryHandler(IScheduleResolverService resolverService)
    : IQueryHandler<GetResolvedEmployeeScheduleQuery, ErrorOr<List<ResolvedScheduleDayDto>>>
{
    public async Task<ErrorOr<List<ResolvedScheduleDayDto>>> HandleAsync(
        GetResolvedEmployeeScheduleQuery request,
        CancellationToken cancellationToken)
    {
        var resolvedList = await resolverService.ResolveRangeAsync(
            request.EmployeeId,
            request.StartDate,
            request.EndDate,
            cancellationToken);

        return resolvedList;
    }
}


