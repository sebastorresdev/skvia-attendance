using ErrorOr;
using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Skvia.Erp.Application.Common.Security;
using Skvia.Erp.Application.Common.Security.Roles;
using Skvia.Erp.Domain.Branches;
using Skvia.Erp.Domain.Workplaces;
using Skvia.Erp.Domain.Identity;

namespace Skvia.Erp.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();

        ApplicationDbContextInitialiser initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly SeedOptions _seedOptions;

    public ApplicationDbContextInitialiser(
        ILogger<ApplicationDbContextInitialiser> logger,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IOptions<SeedOptions> seedOptions)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _seedOptions = seedOptions.Value;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsRelational())
            {
                await _context.Database.MigrateAsync();
            }
            else
            {
                await _context.Database.EnsureCreatedAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (!_seedOptions.Enabled)
        {
            _logger.LogInformation("Seed de datos deshabilitado por configuración.");
            return;
        }

        var adminUserName = !string.IsNullOrWhiteSpace(_seedOptions.AdminUserName)
            ? _seedOptions.AdminUserName
            : Environment.GetEnvironmentVariable("SKVIA_ADMIN_USERNAME") ?? "admin";

        var adminEmail = !string.IsNullOrWhiteSpace(_seedOptions.AdminEmail)
            ? _seedOptions.AdminEmail
            : Environment.GetEnvironmentVariable("SKVIA_ADMIN_EMAIL") ?? "admin@skvia.pe";

        var adminPassword = !string.IsNullOrWhiteSpace(_seedOptions.AdminPassword)
            ? _seedOptions.AdminPassword
            : Environment.GetEnvironmentVariable("SKVIA_ADMIN_PASSWORD");

        if (string.IsNullOrWhiteSpace(adminPassword))
        {
            _logger.LogWarning("Seed habilitado pero sin contraseña configurada. Para activar el seed usa la variable de entorno SKVIA_ADMIN_PASSWORD o la sección Seed:AdminPassword. Se omite el seed.");
            return;
        }

        if (string.IsNullOrWhiteSpace(adminUserName) || string.IsNullOrWhiteSpace(adminEmail))
        {
            _logger.LogWarning("Seed habilitado con configuración incompleta de usuario administrador. Se omite el seed.");
            return;
        }

        // Default branches
        var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Code == "SKVIA_01");
        if (branch is null)
        {
            branch = Branch.Create("SKVIA_01", "Sede Central - San Isidro", "Av. Javier Prado Este 1230, San Isidro, Lima");
            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();
        }
        else if (branch.Name == "Sede principal")
        {
            branch.Update("SKVIA_01", "Sede Central - San Isidro", "Av. Javier Prado Este 1230, San Isidro, Lima");
        }

        var branch2 = await _context.Branches.FirstOrDefaultAsync(b => b.Code == "SKVIA_02");
        if (branch2 is null)
        {
            branch2 = Branch.Create("SKVIA_02", "Sede Sur - Arequipa", "Av. Ejército 742, Yanahuara, Arequipa");
            _context.Branches.Add(branch2);
            await _context.SaveChangesAsync();
        }
        else if (branch2.Name == "Sede base")
        {
            branch2.Update("SKVIA_02", "Sede Sur - Arequipa", "Av. Ejército 742, Yanahuara, Arequipa");
        }

        var branch3 = await _context.Branches.FirstOrDefaultAsync(b => b.Code == "SKVIA_03");
        if (branch3 is null)
        {
            branch3 = Branch.Create("SKVIA_03", "Sede Norte - Trujillo", "Calle Real 450, Trujillo");
            _context.Branches.Add(branch3);
            await _context.SaveChangesAsync();
        }

        // Default Workplaces
        var workplace1 = await _context.Workplaces.FirstOrDefaultAsync(w => w.Code == "WP_01");
        if (workplace1 is null)
        {
            workplace1 = Workplace.Create("WP_01", "Lugar Central - San Isidro", "America/Lima", -12.0931, -77.0305, 200, "Av. Javier Prado Este 1230, San Isidro, Lima");
            _context.Workplaces.Add(workplace1);
            await _context.SaveChangesAsync();
        }

        var workplace2 = await _context.Workplaces.FirstOrDefaultAsync(w => w.Code == "WP_02");
        if (workplace2 is null)
        {
            workplace2 = Workplace.Create("WP_02", "Lugar Sur - Arequipa", "America/Lima", -16.3988, -71.5350, 300, "Av. Ejército 742, Yanahuara, Arequipa");
            _context.Workplaces.Add(workplace2);
            await _context.SaveChangesAsync();
        }

        // Seed Roles Funcionales del Sistema
        var allPermissions = GetPermissionsFromConstants();

        // 1. SuperAdmin (Desarrollador / Admin técnico)
        await SeedRoleWithPermissionsAsync(
            Roles.SuperAdmin,
            "Rol de Desarrollador / Administrador técnico con acceso total al sistema",
            allPermissions);

        // 2. HRAdmin (Administrador de Recursos Humanos)
        var hrAdminPermissions = allPermissions.Where(p => !p.StartsWith("Permissions.Roles.")).ToList();
        await SeedRoleWithPermissionsAsync(
            Roles.HRAdmin,
            "Rol de Gestión Integral de Recursos Humanos, Empleados, Horarios y Asistencias",
            hrAdminPermissions);

        // 3. Supervisor (Jefe de Área / Supervisor de Sede)
        var supervisorPermissions = new List<string>
        {
            Permission.Employee.View,
            Permission.Department.View,
            Permission.Schedule.View,
            Permission.EmployeeSchedule.View,
            Permission.Attendance.View,
            Permission.Attendance.Export,
            Permission.Justification.View,
            Permission.Justification.Approve,
            Permission.Dashboard.View
        };
        await SeedRoleWithPermissionsAsync(
            Roles.Supervisor,
            "Rol de Supervisor / Jefe de Área para consulta y aprobación de asistencias/justificaciones de su equipo",
            supervisorPermissions);

        // 4. Empleado (Personal General / Autoservicio)
        var employeePermissions = new List<string>
        {
            Permission.Attendance.View,
            Permission.Attendance.Register,
            Permission.Justification.Create,
            Permission.Justification.View,
            Permission.EmployeeSchedule.View
        };
        await SeedRoleWithPermissionsAsync(
            Roles.Employee,
            "Rol de Empleado para autoservicio, consulta de marcaciones, turnos y solicitudes de justificación",
            employeePermissions);

        // 5. KioskDevice (Dispositivo de Marcación)
        var kioskPermissions = new List<string>
        {
            Permission.Attendance.Register,
            Permission.KioskDevices.View
        };
        await SeedRoleWithPermissionsAsync(
            Roles.KioskDevice,
            "Rol para dispositivos kiosco y terminales de marcación en sedes",
            kioskPermissions);


        // Default users
        ApplicationUser? administrator = await _userManager.FindByNameAsync(adminUserName);

        if (administrator is null)
        {
            administrator = new ApplicationUser
            {
                DisplayName = "Admin",
                UserName = adminUserName,
                Email = adminEmail,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
            };

            await _userManager.CreateAsync(
                administrator,
                adminPassword);

            administrator.BranchUsers.Add(new BranchUser { BranchId = branch.Id, UserId = administrator.Id });

            await _userManager.AddToRoleAsync(
                administrator,
                Roles.Administrator);
        }

        var claim2 = new Claim("permissions", "Permissions.Branches.Create");
        await _userManager.AddClaimAsync(administrator, claim2);

        await _context.SaveChangesAsync();
    }

    private async Task SeedRoleWithPermissionsAsync(string roleName, string description, IEnumerable<string> permissions)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            role = new ApplicationRole
            {
                Name = roleName,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow
            };
            await _roleManager.CreateAsync(role);
        }

        var existingClaims = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!existingClaims.Any(c => c.Type == "permissions" && c.Value == permission))
            {
                await _roleManager.AddClaimAsync(role, new Claim("permissions", permission));
            }
        }
    }


    /// <summary>
    /// Extrae de forma dinámica todas las constantes de permisos, lee su atributo [Description] 
    /// y mapea los objetos listos para impactar en la base de datos.
    /// </summary>
    private static List<string> GetPermissionsFromConstants()
    {
        var permissions = new List<string>();

        // Obtiene todas las clases anidadas (Branches, Users, etc.) dentro de Permissions
        var modules = typeof(Permission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var module in modules)
        {
            // Filtra solo los campos constantes de tipo string
            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(string));

            foreach (var fi in fields)
            {
                var codeValue = fi.GetRawConstantValue()?.ToString();
                if (string.IsNullOrEmpty(codeValue)) continue;

                // Buscamos si la constante tiene el atributo [Description] encima
                var descriptionAttribute = fi.GetCustomAttribute<DescriptionAttribute>();

                // Si lo tiene usa el texto amigable (ej: "Crear Sucursal"), si no, usa el nombre del campo técnico
                var amigableName = descriptionAttribute?.Description ?? fi.Name;

                permissions.Add(codeValue);
            }
        }

        return permissions;
    }
}


