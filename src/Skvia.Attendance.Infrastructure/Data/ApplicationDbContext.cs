using System.Reflection;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Employees;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Domain.EmployeeSchedules;

namespace Skvia.Attendance.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
        ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
        ApplicationRoleClaim, ApplicationUserToken>(options), IApplicationDbContext
{
    public DbSet<BranchUser> BranchUsers => Set<BranchUser>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Skvia.Attendance.Domain.EmployeeSchedules.EmployeeSchedule> EmployeeSchedules => Set<Skvia.Attendance.Domain.EmployeeSchedules.EmployeeSchedule>();
    public DbSet<Skvia.Attendance.Domain.Schedules.Schedule> Schedules => Set<Skvia.Attendance.Domain.Schedules.Schedule>();
    public DbSet<ApplicationUserRole> ApplicationUserRole => Set<ApplicationUserRole>();
    public override DatabaseFacade Database => base.Database;
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
