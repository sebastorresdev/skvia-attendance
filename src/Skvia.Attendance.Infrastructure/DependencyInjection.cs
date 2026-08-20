using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Common;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Data;
using Skvia.Attendance.Infrastructure.Data.Interceptors;
using Skvia.Attendance.Infrastructure.Security.CurrentUserProvider;
using Skvia.Attendance.Infrastructure.Services;

namespace Skvia.Attendance.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        // Auditorias
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddSingleton<IClock, SystemClock>();
        builder.Services.AddSingleton<ITimeZoneProvider, SystemTimeZoneProvider>();
        builder.Services.AddSingleton<IKioskPairingService, KioskPairingService>();
        builder.Services.Configure<SeedOptions>(builder.Configuration.GetSection(SeedOptions.SectionName));

        var connectionString = builder.Configuration.GetConnectionString("skvia-attendance-db");
        var isTestingEnvironment = builder.Environment.IsEnvironment("Testing")
            || string.Equals(builder.Configuration["ASPNETCORE_ENVIRONMENT"], "Testing", StringComparison.OrdinalIgnoreCase)
            || string.Equals(builder.Configuration["DOTNET_ENVIRONMENT"], "Testing", StringComparison.OrdinalIgnoreCase)
            || builder.Configuration.GetValue<bool>("UseInMemoryDatabase");
        var useInMemory = isTestingEnvironment || string.IsNullOrWhiteSpace(connectionString);

        // 2. Registro clásico adaptado con las convenciones necesarias
        builder.Services.AddDbContext<ApplicationDbContext>((sp, opt) =>
        {
            var interceptor = sp.GetRequiredService<ISaveChangesInterceptor>();

            if (useInMemory)
            {
                opt.UseInMemoryDatabase("skvia-testing-db");
            }
            else
            {
                opt.UseNpgsql(connectionString).AddInterceptors(interceptor);
            }

            opt.UseSnakeCaseNamingConvention();
            opt.AddInterceptors(interceptor);
        });

        if (!useInMemory)
        {
            // Aspire
            builder.EnrichNpgsqlDbContext<ApplicationDbContext>();
        }

        // Database
        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        // Security
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        // Authentication
        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        // Authorization
        builder.Services.AddAuthorizationBuilder();

        // Identity
        builder.Services.AddIdentityCore<ApplicationUser>(options =>
        {
            // Password policy
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;

            // Lockout policy
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            options.Lockout.MaxFailedAccessAttempts = 5;

            // User settings
            options.User.RequireUniqueEmail = true;

            // Sign-in settings
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddRoles<ApplicationRole>()
        .AddSignInManager()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Services
        builder.Services.AddScoped<IUserPermissionService, UserPermissionService>();
        builder.Services.AddScoped<IUserAccountService, IdentityUserAccountService>();
        builder.Services.AddScoped<IRoleService, IdentityRoleService>();
        builder.Services.AddScoped<IAttendanceExcelExporter, AttendanceExcelExporter>();
        builder.Services.AddScoped<IScheduleResolverService, ScheduleResolverService>();
        
        // Repositories
        builder.Services.AddScoped<Skvia.Attendance.Domain.EmployeeSchedules.IEmployeeScheduleRepository, Skvia.Attendance.Infrastructure.Data.Repositories.EmployeeScheduleRepository>();
        builder.Services.AddScoped<Skvia.Attendance.Domain.Schedules.IScheduleRepository, Skvia.Attendance.Infrastructure.Data.Repositories.ScheduleRepository>();

        return builder;

    }
}
