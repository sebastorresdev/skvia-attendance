using System.Reflection;

using Skvia.Attendance.Api.Endpoints;

namespace Skvia.Attendance.Api.Common.Extensions;

public static class EndpointExtensions
{
    public static WebApplication MapEndpoints(this WebApplication app, Assembly assembly)
    {
        var endpointTypes = assembly.DefinedTypes
            .Where(t => t is { IsAbstract: false, IsInterface: false }
                        && t.IsAssignableTo(typeof(IEndpoint)));

        var grouped = endpointTypes.GroupBy(t =>
        {
            var ns = t.Namespace ?? "";
            var segments = ns.Split('.');
            return segments[^1].ToLower();
        });

        foreach (var group in grouped)
        {
            var routeGroup = app.MapGroup($"/api/v1/{group.Key}")
                .WithTags(group.Key)
                .RequireRateLimiting("DefaultApi");

            foreach (var type in group)
            {
                var method = type.GetMethod("Map");
                method?.Invoke(null, [routeGroup]);
            }
        }

        app.MapGet("/api/health/live", () => Results.Ok(new { status = "Alive" }))
            .WithName("LiveHealth")
            .WithTags("Health")
            .AllowAnonymous();

        app.MapHealthChecks("/api/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("ready"),
            ResponseWriter = WriteHealthResponse
        })
            .WithName("ReadyHealth")
            .WithTags("Health")
            .AllowAnonymous();

        app.MapHealthChecks("/api/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("live"),
            ResponseWriter = WriteHealthResponse
        })
            .WithName("LiveHealthChecks")
            .WithTags("Health")
            .AllowAnonymous();

        return app;
    }

    private static Task WriteHealthResponse(HttpContext context, Microsoft.Extensions.Diagnostics.HealthChecks.HealthReport report)
    {
        context.Response.ContentType = "application/json";
        return context.Response.WriteAsJsonAsync(new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration,
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration
            })
        });
    }
}
