using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Application.Common.Messaging;
using Skvia.Attendance.Domain.EmployeeSchedules;
using Skvia.Attendance.Domain.Justifications;

namespace Skvia.Attendance.Application.Features.Attendances.Queries.GetMonthlySummary;

public class GetMonthlySummaryQueryHandler(
    IApplicationDbContext dbContext) : IQueryHandler<GetMonthlySummaryQuery, ErrorOr<MonthlySummaryResponse>>
{
    public async Task<ErrorOr<MonthlySummaryResponse>> HandleAsync(GetMonthlySummaryQuery query, CancellationToken cancellationToken)
    {
        var startDate = new DateOnly(query.Year, query.Month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        // Fetch employees
        var employeesQuery = dbContext.Employees
            .AsNoTracking();

        if (query.BranchId.HasValue)
        {
            employeesQuery = employeesQuery.Where(e => e.MainBranchId == query.BranchId.Value);
        }

        var employees = await employeesQuery
            .Select(e => new
            {
                e.Id,
                e.Code,
                e.FirstName,
                e.LastName,
                e.MainBranchId,
                BranchName = e.MainBranchId.HasValue ? dbContext.Branches.Where(b => b.Id == e.MainBranchId.Value).Select(b => b.Name).FirstOrDefault() ?? "N/A" : "N/A"
            })
            .ToListAsync(cancellationToken);

        // Fetch schedules for the month
        var schedules = await dbContext.EmployeeSchedules
            .AsNoTracking()
            .Where(s => s.Date >= startDate && s.Date <= endDate)
            .ToListAsync(cancellationToken);

        // Fetch attendances for the month
        var attendances = await dbContext.Attendances
            .AsNoTracking()
            .Where(a => a.Date >= startDate && a.Date <= endDate)
            .ToListAsync(cancellationToken);

        // Fetch approved justifications for the month
        var justifications = await dbContext.Justifications
            .AsNoTracking()
            .Where(j => j.Date >= startDate && j.Date <= endDate && j.Status == JustificationStatus.Approved)
            .ToListAsync(cancellationToken);

        var items = new List<EmployeeMonthlySummaryItemDto>();

        foreach (var emp in employees)
        {
            var empSchedules = schedules.Where(s => s.EmployeeId == emp.Id).ToList();
            var empAttendances = attendances.Where(a => a.EmployeeId == emp.Id).ToList();
            var empJustifications = justifications.Where(j => j.EmployeeId == emp.Id).ToList();

            int workDaysScheduled = empSchedules.Count(s => s.DayType == ScheduleDayType.WorkDay || s.DayType == ScheduleDayType.MakeUpDay);
            int daysWorked = empAttendances.Count;
            int daysOff = empSchedules.Count(s => s.DayType == ScheduleDayType.DayOff);
            int vacationDays = empSchedules.Count(s => s.DayType == ScheduleDayType.Vacation);
            int medicalLeaveDays = empSchedules.Count(s => s.DayType == ScheduleDayType.MedicalLeave);

            // Calculate absences: Scheduled WorkDay where Date <= today and no attendance checkin
            int totalScheduledWorkDaysPassed = empSchedules.Count(s => s.DayType == ScheduleDayType.WorkDay);
            int totalAbsences = Math.Max(0, totalScheduledWorkDaysPassed - daysWorked);

            int justifiedAbsences = empJustifications.Count(j => j.Type == JustificationType.Absence);
            int unjustifiedAbsences = Math.Max(0, totalAbsences - justifiedAbsences);

            int totalLateMinutes = empAttendances.Sum(a => a.MinutesLate);

            // Calculate late minutes justified by approved tardiness justifications
            int justifiedLateMinutes = 0;
            var justifiedTardinessDates = empJustifications
                .Where(j => j.Type == JustificationType.Tardiness)
                .Select(j => j.Date)
                .ToHashSet();

            justifiedLateMinutes = empAttendances
                .Where(a => a.IsLate && justifiedTardinessDates.Contains(a.Date))
                .Sum(a => a.MinutesLate);

            int totalOvertimeMinutes = empAttendances.Sum(a => a.OvertimeMinutes);

            items.Add(new EmployeeMonthlySummaryItemDto(
                emp.Id,
                emp.Code,
                $"{emp.LastName}, {emp.FirstName}",
                emp.BranchName,
                workDaysScheduled,
                daysWorked,
                daysOff,
                vacationDays,
                medicalLeaveDays,
                unjustifiedAbsences,
                justifiedAbsences,
                totalLateMinutes,
                justifiedLateMinutes,
                totalOvertimeMinutes
            ));
        }

        return new MonthlySummaryResponse(query.Year, query.Month, items.Count, items);
    }
}
