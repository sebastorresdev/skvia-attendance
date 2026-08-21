namespace Skvia.Erp.Domain.EmployeeSchedules;

public interface IEmployeeScheduleRepository
{
    Task AddRangeAsync(IEnumerable<EmployeeSchedule> schedules, CancellationToken cancellationToken = default);
    Task<List<EmployeeSchedule>> GetByEmployeeAndDateRangeAsync(Guid employeeId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<EmployeeSchedule> schedules, CancellationToken cancellationToken = default);
}

