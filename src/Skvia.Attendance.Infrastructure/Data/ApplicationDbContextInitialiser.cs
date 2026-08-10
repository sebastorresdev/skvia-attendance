using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Skvia.Attendance.Application.Common.Security;
using Skvia.Attendance.Application.Common.Security.Roles;
using Skvia.Attendance.Domain.Branches;
using Skvia.Attendance.Domain.Workplaces;
using Skvia.Attendance.Domain.Identity;

namespace Skvia.Attendance.Infrastructure.Data;

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
            _logger.LogWarning("No se configuró una contraseña para el usuario administrador; se omite el seed.");
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

        // Default roles
        var existingAdminRole = await _roleManager.FindByNameAsync(Roles.Administrator);

        if (existingAdminRole is null)
        {
            existingAdminRole = new ApplicationRole
            {
                Name = Roles.Administrator,
                Description = "Rol de administrador con todos los permisos del sistema",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
            };
            await _roleManager.CreateAsync(existingAdminRole);
        }

        // REGISTRAMOS LOS PERMISOS AL ROL ADMIN
        // Recuperamos los claims actuales para evitar duplicados si el seed vuelve a correr
        var existingClaims = await _roleManager.GetClaimsAsync(existingAdminRole);

        foreach (var permission in GetPermissionsFromConstants())
        {
            if (!existingClaims.Any(c => c.Type == "permissions" && c.Value == permission))
            {
                var claim = new Claim("permissions", permission);
                await _roleManager.AddClaimAsync(existingAdminRole, claim);
            }
        }

        var basicRole = await _roleManager.FindByNameAsync("Usuario");
        if (basicRole is null)
        {
            basicRole = new ApplicationRole
            {
                Name = "Usuario",
                Description = "Rol básico del sistema con accesos limitados",
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
            };
            await _roleManager.CreateAsync(basicRole);
        }
        else
        {
            bool needUpdate = false;
            if (basicRole.CreatedAt == default)
            {
                basicRole.CreatedAt = DateTime.UtcNow;
                needUpdate = true;
            }
            if (basicRole.LastModifiedAt == default)
            {
                basicRole.LastModifiedAt = DateTime.UtcNow;
                needUpdate = true;
            }
            if (string.IsNullOrWhiteSpace(basicRole.Description))
            {
                basicRole.Description = "Rol básico del sistema con accesos limitados";
                needUpdate = true;
            }
            if (needUpdate)
            {
                await _roleManager.UpdateAsync(basicRole);
            }
        }

        var existingBasicClaims = await _roleManager.GetClaimsAsync(basicRole);
        if (!existingBasicClaims.Any(c => c.Type == "permissions" && c.Value == Permission.System.Access))
        {
            var claim = new Claim("permissions", Permission.System.Access);
            await _roleManager.AddClaimAsync(basicRole, claim);
        }

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
