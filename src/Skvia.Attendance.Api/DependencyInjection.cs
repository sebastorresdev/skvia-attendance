using Skvia.Attendance.Api.Common.Exceptions;
using Skvia.Attendance.Api.Common.OpenApi;

namespace Skvia.Attendance.Api;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddWebServices(this IHostApplicationBuilder builder)
    {
        // Errors
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();
        // CORS
        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod());
        });
        // OpenAPI
        builder.Services.AddOpenApi(opt =>
        {
            opt.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });
        return builder;
    }
}
