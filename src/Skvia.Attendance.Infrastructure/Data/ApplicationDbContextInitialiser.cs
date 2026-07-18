using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Skvia.Attendance.Application.Common.Security.Roles;
using Skvia.Attendance.Infrastructure.Identity;

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
        // Default roles
        var administratorRole = new ApplicationRole
        {
            Name = Roles.Administrator
        };

        if (!await _roleManager.RoleExistsAsync(administratorRole.Name!))
        {
            await _roleManager.CreateAsync(administratorRole);
        }

        // Default users
        ApplicationUser? administrator = await _userManager.FindByNameAsync("administrator@localhost");

        if (administrator is null)
        {
            administrator = new ApplicationUser
            {
                DisplayName = "Admin",
                UserName = "administrator@localhost",
                Email = "administrator@localhost"
            };

            await _userManager.CreateAsync(
                administrator,
                "Administrator1!");

            await _userManager.AddToRoleAsync(
                administrator,
                Roles.Administrator);
        }

        await _context.SaveChangesAsync();
    }
}
