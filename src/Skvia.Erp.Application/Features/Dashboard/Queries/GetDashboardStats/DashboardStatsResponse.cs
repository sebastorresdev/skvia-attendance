namespace Skvia.Erp.Application.Features.Dashboard.Queries.GetDashboardStats;

public record DashboardStatsResponse(
    int TotalActiveEmployees,
    int TodayCheckIns,
    int TodayLateCheckIns,
    int TodayOnBreak,
    int TodayEstimatedAbsences,
    List<WeeklyTrendItemDto> WeeklyTrend,
    List<BranchAttendanceItemDto> BranchBreakdown,
    List<RecentActivityItemDto> RecentActivities);

public record WeeklyTrendItemDto(string DateLabel, int OnTimeCount, int LateCount);
public record BranchAttendanceItemDto(Guid BranchId, string BranchName, int CheckInsCount);
public record RecentActivityItemDto(
    Guid AttendanceId,
    string EmployeeName,
    string EmployeeCode,
    string? PhotoUrl,
    string BranchName,
    DateTimeOffset CheckInTime,
    bool IsLate,
    int MinutesLate);

