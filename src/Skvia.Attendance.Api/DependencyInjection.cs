using Asp.Versioning;
using Microsoft.Extensions.Diagnostics.HealthChecks;

using Skvia.Attendance.Api.Common.Exceptions;
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
            .AddCheck("database", () => HealthCheckResult.Healthy(), tags: ["ready"]);

        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });

        builder.Services.AddOpenApi(opt =>
        {
            opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return builder;
    }
}
