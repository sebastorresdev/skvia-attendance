using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Common;
using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Application.Features.Dashboard.Queries.GetScheduleAlerts;

public class GetScheduleAlertsQueryHandler(
    IApplicationDbContext dbContext,
    IClock clock) : IQueryHandler<GetScheduleAlertsQuery, ErrorOr<List<ScheduleAlertDto>>>
{
    public async Task<ErrorOr<List<ScheduleAlertDto>>> HandleAsync(GetScheduleAlertsQuery query, CancellationToken cancellationToken)
    {
        // Get local date based on standard timezone or system timezone
        // For simplicity we use standard UTC to Local or just System Local since it's a general dashboard.
        var today = DateOnly.FromDateTime(clock.UtcNow.DateTime); // Ideally converted to local branch tz, but this is a global alert
        var thresholdDate = today.AddDays(15);

        // Find all active employees
        var activeEmployees = await dbContext.Employees
            .Where(e => e.Status == EmployeeStatus.Active)
            .Select(e => new { e.Id, e.Code, e.FirstName, e.LastName })
            .ToListAsync(cancellationToken);

        var employeeIds = activeEmployees.Select(e => e.Id).ToList();

        // Find the maximum scheduled date for these employees
        var maxSchedules = await dbContext.EmployeeSchedules
            .Where(s => employeeIds.Contains(s.EmployeeId))
            .GroupBy(s => s.EmployeeId)
            .Select(g => new { EmployeeId = g.Key, MaxDate = g.Max(s => s.Date) })
            .ToListAsync(cancellationToken);

        var maxSchedulesDict = maxSchedules.ToDictionary(m => m.EmployeeId, m => m.MaxDate);

        var alerts = new List<ScheduleAlertDto>();

        foreach (var emp in activeEmployees)
        {
            maxSchedulesDict.TryGetValue(emp.Id, out var lastDate);

            // If they have no schedule at all, or their last schedule is within the next 15 days
            if (lastDate == default || lastDate <= thresholdDate)
            {
                alerts.Add(new ScheduleAlertDto(emp.Id, emp.Code, $"{emp.FirstName} {emp.LastName}", lastDate == default ? null : lastDate));
            }
        }

        return alerts.OrderBy(a => a.LastScheduleDate).ToList();
    }
}


