using System.Reflection;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Departments;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.EmployeeSchedules;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Domain.Schedules;
using Skvia.Attendance.Domain.Kiosks;
using Skvia.Attendance.Domain.Workplaces;

namespace Skvia.Attendance.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken>(options), IApplicationDbContext
{
    public DbSet<BranchUser> BranchUsers => Set<BranchUser>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeeSchedule> EmployeeSchedules => Set<EmployeeSchedule>();
    public DbSet<ScheduleException> ScheduleExceptions => Set<ScheduleException>();
    public DbSet<Schedule> Schedules => Set<Schedule>();
    public DbSet<ApplicationUserRole> ApplicationUserRole => Set<ApplicationUserRole>();
    public override DatabaseFacade Database => base.Database;

    public DbSet<Domain.Attendances.Attendance> Attendances => Set<Domain.Attendances.Attendance>();
    public DbSet<KioskDevice> KioskDevices => Set<KioskDevice>();
    public DbSet<Workplace> Workplaces => Set<Workplace>();
    public DbSet<Domain.Justifications.Justification> Justifications => Set<Domain.Justifications.Justification>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
