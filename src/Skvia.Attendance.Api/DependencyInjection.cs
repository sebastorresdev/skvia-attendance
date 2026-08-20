using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Skvia.Attendance.Api.Common.Exceptions;
using Skvia.Attendance.Api.Common.Health;
using Skvia.Attendance.Api.Common.OpenApi;

namespace Skvia.Attendance.Api;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });

        builder.Services.AddHealthChecks()
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: ["live"])
            .AddCheck<DatabaseHealthCheck>("database", tags: ["ready"]);

        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:4200", "http://localhost:8080"];

        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("AllowAll", policy =>
            {
                if (allowedOrigins.Contains("*"))
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                }
                else
                {
                    policy.SetIsOriginAllowed(origin =>
                    {
                        if (builder.Environment.IsDevelopment())
                        {
                            if (Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                                (uri.Host is "localhost" or "127.0.0.1"))
                            {
                                return true;
                            }
                        }
                        return allowedOrigins.Contains(origin, StringComparer.OrdinalIgnoreCase);
                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
                }
            });
        });

        builder.Services.AddRequestTimeouts(options =>
        {
            options.DefaultPolicy = new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
            {
                Timeout = TimeSpan.FromSeconds(15),
                TimeoutStatusCode = StatusCodes.Status504GatewayTimeout
            };
            options.AddPolicy("UploadPolicy", new Microsoft.AspNetCore.Http.Timeouts.RequestTimeoutPolicy
            {
                Timeout = TimeSpan.FromSeconds(60),
                TimeoutStatusCode = StatusCodes.Status504GatewayTimeout
            });
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddFixedWindowLimiter("StrictLogin", opt =>
            {
                opt.PermitLimit = 5;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueLimit = 0;
            });
            options.AddFixedWindowLimiter("DefaultApi", opt =>
            {
                opt.PermitLimit = 120;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueLimit = 0;
            });
        });

        builder.Services.AddOutputCache(options =>
        {
            options.AddBasePolicy(b => b.Expire(TimeSpan.FromSeconds(60)));
            options.AddPolicy("CatalogCache", b => b.Expire(TimeSpan.FromMinutes(5)).Tag("catalogs"));
        });

        builder.Services.AddOpenApi(opt =>
        {
            opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return builder;
    }
}
