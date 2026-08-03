using Skvia.Attendance.Domain.Schedules;

namespace Skvia.Attendance.Infrastructure.Data.Repositories;

public sealed class ScheduleRepository(ApplicationDbContext context) : IScheduleRepository
{
    public async Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Set<Schedule>().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<List<Schedule>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Set<Schedule>().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default)
    {
        await context.Set<Schedule>().AddAsync(schedule, cancellationToken);
    }

    public void Update(Schedule schedule)
    {
        context.Set<Schedule>().Update(schedule);
    }

    public void Remove(Schedule schedule)
    {
        context.Set<Schedule>().Remove(schedule);
    }
}
