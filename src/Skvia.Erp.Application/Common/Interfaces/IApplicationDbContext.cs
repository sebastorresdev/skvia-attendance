using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Skvia.Erp.Domain.Branches;
using Skvia.Erp.Domain.Departments;
using Skvia.Erp.Domain.Employees;
using Skvia.Erp.Domain.Identity;
using Skvia.Erp.Domain.Kiosks;
using Skvia.Erp.Domain.Workplaces;

namespace Skvia.Erp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<BranchUser> BranchUsers { get; }
    DbSet<Branch> Branches { get; }
    DbSet<Department> Departments { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Skvia.Erp.Domain.EmployeeSchedules.EmployeeSchedule> EmployeeSchedules { get; }
    DbSet<Skvia.Erp.Domain.EmployeeSchedules.ScheduleException> ScheduleExceptions { get; }
    DbSet<Skvia.Erp.Domain.Schedules.Schedule> Schedules { get; }
    DbSet<Skvia.Erp.Domain.Attendances.Attendance> Attendances { get; }
    DbSet<KioskDevice> KioskDevices { get; }
    DbSet<Workplace> Workplaces { get; }
    DbSet<Skvia.Erp.Domain.Justifications.Justification> Justifications { get; }

    DbSet<ApplicationUserRole> ApplicationUserRole { get; }
    DatabaseFacade Database { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

