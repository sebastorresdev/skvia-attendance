using Microsoft.EntityFrameworkCore;
using System.Reflection;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Domain.Branches;
using Skvia.Erp.Domain.Departments;
using Skvia.Erp.Domain.Employees;
using Skvia.Erp.Domain.EmployeeSchedules;
using Skvia.Erp.Domain.Identity;
using Skvia.Erp.Domain.Schedules;
using Skvia.Erp.Domain.Kiosks;
using Skvia.Erp.Domain.Workplaces;

namespace Skvia.Erp.Infrastructure.Data;

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


