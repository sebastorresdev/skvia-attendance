using Microsoft.EntityFrameworkCore.Infrastructure;

using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BranchUser> BranchUsers { get; }
    DbSet<Branch> Branches { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Skvia.Attendance.Domain.EmployeeSchedules.EmployeeSchedule> EmployeeSchedules { get; }
    DbSet<Skvia.Attendance.Domain.Schedules.Schedule> Schedules { get; }

    DbSet<ApplicationUserRole> ApplicationUserRole { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
