namespace Skvia.Attendance.Domain.Schedules;

public interface IScheduleRepository
{
    Task<Schedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Schedule>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Schedule schedule, CancellationToken cancellationToken = default);
    void Update(Schedule schedule);
    void Remove(Schedule schedule);
}
