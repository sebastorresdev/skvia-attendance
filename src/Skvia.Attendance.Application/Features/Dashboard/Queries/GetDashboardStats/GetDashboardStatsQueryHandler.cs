using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Employees;

namespace Skvia.Attendance.Application.Features.Dashboard.Queries.GetDashboardStats;

public class GetDashboardStatsQueryHandler(
    IApplicationDbContext dbContext,
    IClock clock,
    ITimeZoneProvider timeZoneProvider) : IQueryHandler<GetDashboardStatsQuery, ErrorOr<DashboardStatsResponse>>
{
    public async Task<ErrorOr<DashboardStatsResponse>> HandleAsync(GetDashboardStatsQuery query, CancellationToken cancellationToken)
    {
        // 1. Determine Local Date today
        var limaZone = timeZoneProvider.GetTimeZone("America/Lima");
        var localNow = TimeZoneInfo.ConvertTime(clock.UtcNow, limaZone);
        var today = DateOnly.FromDateTime(localNow.DateTime);

        // 2. Active Employees Count
        var activeEmployeesQuery = dbContext.Employees
            .AsNoTracking()
            .Where(e => e.Status == EmployeeStatus.Active);

        if (query.BranchId.HasValue)
        {
            activeEmployeesQuery = activeEmployeesQuery.Where(e => e.MainBranchId == query.BranchId.Value);
        }

        var totalActiveEmployees = await activeEmployeesQuery.CountAsync(cancellationToken);

        // 3. Attendances Today Query
        var todayAttendancesQuery = dbContext.Attendances
            .AsNoTracking()
            .Where(a => a.Date == today);

        if (query.BranchId.HasValue)
        {
            todayAttendancesQuery = todayAttendancesQuery.Where(a => a.CheckInWorkplaceId == query.BranchId.Value);
        }

        var todayCheckIns = await todayAttendancesQuery.CountAsync(cancellationToken);
        var todayLateCheckIns = await todayAttendancesQuery.Where(a => a.IsLate).CountAsync(cancellationToken);
        var todayOnBreak = await todayAttendancesQuery.Where(a => a.BreakStart.HasValue && !a.BreakEnd.HasValue).CountAsync(cancellationToken);

        // 4. Estimated Absences Today
        var todaySchedulesQuery = dbContext.EmployeeSchedules
            .AsNoTracking()
            .Where(s => s.Date == today && s.DayType == Domain.EmployeeSchedules.ScheduleDayType.WorkDay);

        if (query.BranchId.HasValue)
        {
            todaySchedulesQuery = todaySchedulesQuery.Where(s => s.Employee.MainBranchId == query.BranchId.Value);
        }

        var todaySchedulesCount = await todaySchedulesQuery.CountAsync(cancellationToken);
        int todayEstimatedAbsences = Math.Max(0, todaySchedulesCount - todayCheckIns);

        // 5. Weekly Trend (Last 7 Days)
        var startDate = today.AddDays(-6);
        var weeklyAttendancesQuery = dbContext.Attendances
            .AsNoTracking()
            .Where(a => a.Date >= startDate && a.Date <= today);

        if (query.BranchId.HasValue)
        {
            weeklyAttendancesQuery = weeklyAttendancesQuery.Where(a => a.CheckInWorkplaceId == query.BranchId.Value);
        }

        var weeklyAttendances = await weeklyAttendancesQuery
            .Select(a => new { a.Date, a.IsLate })
            .ToListAsync(cancellationToken);

        var weeklyTrend = new List<WeeklyTrendItemDto>();
        for (int i = 0; i < 7; i++)
        {
            var date = startDate.AddDays(i);
            var dayRecords = weeklyAttendances.Where(a => a.Date == date).ToList();
            int onTime = dayRecords.Count(a => !a.IsLate);
            int late = dayRecords.Count(a => a.IsLate);
            weeklyTrend.Add(new WeeklyTrendItemDto(date.ToString("dd/MM"), onTime, late));
        }

        // 6. Branch Breakdown Today
        var branchBreakdown = await dbContext.Attendances
            .AsNoTracking()
            .Where(a => a.Date == today)
            .GroupBy(a => new { a.CheckInWorkplaceId, a.CheckInWorkplace.Name })
            .Select(g => new BranchAttendanceItemDto(g.Key.CheckInWorkplaceId, g.Key.Name, g.Count()))
            .ToListAsync(cancellationToken);

        // 7. Recent Activities Today (Top 10)
        var recentActivities = await todayAttendancesQuery
            .OrderByDescending(a => a.CheckIn)
            .Take(10)
            .Select(a => new RecentActivityItemDto(
                a.Id,
                $"{a.Employee.FirstName} {a.Employee.LastName}",
                a.Employee.Code,
                a.Employee.PhotoUrl,
                a.CheckInWorkplace.Name,
                a.CheckIn,
                a.IsLate,
                a.MinutesLate
            ))
            .ToListAsync(cancellationToken);

        return new DashboardStatsResponse(
            totalActiveEmployees,
            todayCheckIns,
            todayLateCheckIns,
            todayOnBreak,
            todayEstimatedAbsences,
            weeklyTrend,
            branchBreakdown,
            recentActivities);
    }
}
