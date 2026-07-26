using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

        await _context.SaveChangesAsync();
    }
}
