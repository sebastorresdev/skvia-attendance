using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Skvia.Erp.Api;
using Skvia.Erp.Application.Common.Constants;
using Skvia.Erp.Domain.Identity;
using Skvia.Erp.Infrastructure.Data;

namespace Skvia.Erp.Application.Tests.Integration;

public class ApiAuthAndErrorTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiAuthAndErrorTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    [Fact]
    public async Task ProtectedEndpoint_WhenNoAuthentication_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/v1/dashboard/schedule-alerts");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WhenBadCredentials_ReturnsUnauthorizedProblemDetails()
    {
        var payload = new { userName = "noexiste", password = "badpass" };

        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsPayload>();
        Assert.NotNull(problem);
        Assert.NotNull(problem!.Title);
    }

    [Fact]
    public async Task ProtectedEndpoint_WhenAuthenticated_ReturnsOk()
    {
        var authClient = _client;
        authClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestScheme", "test-token");

        var response = await authClient.GetAsync("/api/v1/dashboard/schedule-alerts");

        Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UnknownRoute_WhenRequested_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/v1/unknown-route-that-does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:DisableInitialization"] = "true",
                ["UseInMemoryDatabase"] = "true",
                ["ConnectionStrings:skvia-erp-db"] = string.Empty
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll(typeof(DbContextOptions<>));

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("skvia-testing-db");
            });

            services.RemoveAll<IAuthenticationSchemeProvider>();
            services.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
            services.RemoveAll<IPostConfigureOptions<AuthenticationOptions>>();

            services.AddAuthentication("TestScheme")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", _ => { });

            services.PostConfigureAll<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
                options.DefaultScheme = "TestScheme";
            });
        });
    }
}

public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out var authHeader) ||
            string.IsNullOrWhiteSpace(authHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!AuthenticationHeaderValue.TryParse(authHeader, out var parsedHeader) ||
            !string.Equals(parsedHeader.Scheme, "TestScheme", StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.Empty.ToString()),
            new Claim(ClaimTypes.Name, "test-user"),
            new Claim(ClaimTypes.Role, "Usuario"),
            new Claim(CustomClaimTypes.Permission, "Permissions.System.Access")
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public sealed record ProblemDetailsPayload(
    string? Type,
    string? Title,
    int? Status,
    string? Detail,
    string? Instance,
    Dictionary<string, string[]>? Errors);


