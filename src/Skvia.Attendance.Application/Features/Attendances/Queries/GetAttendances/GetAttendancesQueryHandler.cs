using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Messaging;
using ErrorOr;
using System.Linq;

namespace Skvia.Attendance.Application.Features.Attendances.Queries.GetAttendances;

public class GetAttendancesQueryHandler(
    IApplicationDbContext dbContext,
    IScheduleResolverService scheduleResolver) : IQueryHandler<GetAttendancesQuery, ErrorOr<List<AttendanceResponse>>>
{
    public async Task<ErrorOr<List<AttendanceResponse>>> HandleAsync(GetAttendancesQuery query, CancellationToken cancellationToken)
    {
        var queryable = dbContext.Attendances
            .AsNoTracking()
            .Where(a => a.Date >= query.StartDate && a.Date <= query.EndDate);

        if (query.WorkplaceId.HasValue)
        {
            queryable = queryable.Where(a => a.CheckInWorkplaceId == query.WorkplaceId.Value);
        }

        if (query.EmployeeId.HasValue)
        {
            queryable = queryable.Where(a => a.EmployeeId == query.EmployeeId.Value);
        }

        if (!string.IsNullOrWhiteSpace(query.StatusFilter))
        {
            if (query.StatusFilter.Equals("late", StringComparison.OrdinalIgnoreCase))
            {
                queryable = queryable.Where(a => a.IsLate);
            }
            else if (query.StatusFilter.Equals("onTime", StringComparison.OrdinalIgnoreCase))
            {
                queryable = queryable.Where(a => !a.IsLate);
            }
        }

        if (!string.IsNullOrWhiteSpace(query.EmployeeSearch))
        {
            var search = $"%{query.EmployeeSearch.Trim().ToLower()}%";
            queryable = queryable.Where(a =>
                EF.Functions.Like(a.Employee.FirstName.ToLower(), search) ||
                EF.Functions.Like(a.Employee.LastName.ToLower(), search) ||
                EF.Functions.Like(a.Employee.DocumentIdentifier.Number.ToLower(), search) ||
                EF.Functions.Like(a.Employee.Code.ToLower(), search));
        }

        var responseList = await queryable
            .OrderByDescending(a => a.Date)
            .ThenBy(a => a.Employee.LastName)
            .Select(a => new AttendanceResponse(
                a.Id,
                a.EmployeeId,
                a.Employee.FirstName + " " + a.Employee.LastName,
                a.Employee.Code,
                a.CheckInWorkplaceId,
                a.CheckInWorkplace.Name,
                a.Date,
                null, // assignedStartTime
                null, // assignedEndTime
                a.CheckIn,
                a.CheckOut,
                a.MinutesLate,
                a.IsLate
            ))
            .ToListAsync(cancellationToken);

        var employeeIds = responseList.Select(r => r.EmployeeId).Distinct().ToList();
        if (employeeIds.Count > 0)
        {
            var grid = await scheduleResolver.ResolveGridAsync(employeeIds, query.StartDate, query.EndDate, cancellationToken);

            responseList = responseList.Select(r =>
            {
                if (grid.TryGetValue(r.EmployeeId, out var days))
                {
                    var day = days.FirstOrDefault(d => d.Date == r.Date);
                    if (day is not null && day.StartTime.HasValue)
                    {
                        return r with
                        {
                            AssignedStartTime = day.StartTime?.ToTimeSpan(),
                            AssignedEndTime = day.EndTime?.ToTimeSpan()
                        };
                    }
                }
                return r;
            }).ToList();
        }

        return responseList;
    }
}
