using System.ComponentModel;
using System.Reflection;
using System.Security.Claims;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Skvia.Attendance.Application.Common.Security;
using Skvia.Attendance.Application.Common.Security.Roles;
using Skvia.Attendance.Domain.Branches;
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

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            // See https://jasontaylor.dev/ef-core-database-initialisation-strategies
            await _context.Database.EnsureDeletedAsync();
            await _context.Database.EnsureCreatedAsync();
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
        // Default branches
        var branch = Branch.Create("SKVIA_01", "Sede principal");
        var branch2 = Branch.Create("SKVIA_02", "Sede base");

        _context.Branches.Add(branch);
        _context.Branches.Add(branch2);

        // Default roles
        var administratorRole = new ApplicationRole
        {
            Name = Roles.Administrator,
        };

        if (!await _roleManager.RoleExistsAsync(administratorRole.Name))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // REGISTRAMOS LOS PERMISOS AL ROL ADMIN
        foreach (var permission in GetPermissionsFromConstants())
        {
            var claim = new Claim("permissions", permission);
            await _roleManager.AddClaimAsync(administratorRole, claim);
        }

        var basicRole = new ApplicationRole
        {
            Name = "basic",
        };

        if (!await _roleManager.RoleExistsAsync(basicRole.Name))
        {
            await _roleManager.CreateAsync(basicRole);
        }

        // Default users
        ApplicationUser? administrator = await _userManager.FindByNameAsync("admin");

        if (administrator is null)
        {
            administrator = new ApplicationUser
            {
                DisplayName = "Admin",
                UserName = "admin",
                Email = "admin@skvia.pe",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                LastModifiedAt = DateTime.UtcNow,
            };

            await _userManager.CreateAsync(
                administrator,
                "Password123*");

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
        // TODO: revisar la implementacion de los permisos para un PermissionViewModel

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

                //permissions.Add(new Permission
                //{
                //    Code = codeValue, // Tu set nativo de la entidad se encargará de rellenar el NormalizedCode 🚀
                //    Name = amigableName,
                //    Description = $"Permite realizar la acción de {amigableName.ToLower()} en el sistema."
                //});
            }
        }

        return permissions;
    }
}
