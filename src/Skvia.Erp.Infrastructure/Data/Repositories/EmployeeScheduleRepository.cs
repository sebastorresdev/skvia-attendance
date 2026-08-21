using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Domain.EmployeeSchedules;

namespace Skvia.Erp.Infrastructure.Data.Repositories;

public sealed class EmployeeScheduleRepository(ApplicationDbContext context) : IEmployeeScheduleRepository
{
    public async Task AddRangeAsync(IEnumerable<EmployeeSchedule> schedules, CancellationToken cancellationToken = default)
    {
        await context.Set<EmployeeSchedule>().AddRangeAsync(schedules, cancellationToken);
    }

    public async Task<List<EmployeeSchedule>> GetByEmployeeAndDateRangeAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default)
    {
        return await context.Set<EmployeeSchedule>()
            .Where(es => es.EmployeeId == employeeId && es.Date >= startDate && es.Date <= endDate)
            .ToListAsync(cancellationToken);
    }

    public Task DeleteRangeAsync(IEnumerable<EmployeeSchedule> schedules, CancellationToken cancellationToken = default)
    {
        context.Set<EmployeeSchedule>().RemoveRange(schedules);
        return Task.CompletedTask;
    }
}


