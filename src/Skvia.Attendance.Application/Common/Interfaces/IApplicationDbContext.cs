using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Departments;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Domain.Kiosks;
using Skvia.Attendance.Domain.Workplaces;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BranchUser> BranchUsers { get; }
    DbSet<Branch> Branches { get; }
    DbSet<Department> Departments { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Skvia.Attendance.Domain.EmployeeSchedules.EmployeeSchedule> EmployeeSchedules { get; }
    DbSet<Skvia.Attendance.Domain.Schedules.Schedule> Schedules { get; }
    DbSet<Skvia.Attendance.Domain.Attendances.Attendance> Attendances { get; }
    DbSet<KioskDevice> KioskDevices { get; }
    DbSet<Workplace> Workplaces { get; }
    DbSet<Skvia.Attendance.Domain.Justifications.Justification> Justifications { get; }

    DbSet<ApplicationUserRole> ApplicationUserRole { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
