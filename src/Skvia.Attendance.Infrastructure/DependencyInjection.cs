using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Skvia.Attendance.Application.Common.Interfaces;
using Skvia.Attendance.Domain.Identity;
using Skvia.Attendance.Infrastructure.Data;
using Skvia.Attendance.Infrastructure.Data.Interceptors;
using Skvia.Attendance.Infrastructure.Identity;
using Skvia.Attendance.Infrastructure.Security.CurrentUserProvider;

namespace Skvia.Attendance.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        // Auditorias
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        // 2. Registro clásico adaptado con las convenciones necesarias
        builder.Services.AddDbContext<ApplicationDbContext>((sp, opt) =>
        {
            var interceptor = sp.GetRequiredService<ISaveChangesInterceptor>();

            // Nota: El connectionString real ya lo manejará automáticamente el orquestador a nivel de infraestructura
            string? connectionString = builder.Configuration.GetConnectionString("bubba-db"); // Nombre de tu recurso en AppHost

            opt.UseNpgsql(connectionString).AddInterceptors(interceptor);
            opt.UseSnakeCaseNamingConvention();
        });

        // Aspire
        builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

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

        builder.Services.AddScoped<IUserService, UserService>();

        return builder;

    }
}
