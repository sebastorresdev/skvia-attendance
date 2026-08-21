using Skvia.Erp.Infrastructure.Data;
using Skvia.Erp.Api.Common.Extensions;

using Scalar.AspNetCore;


namespace Skvia.Erp.Api;


public static class AppConfig
{
    public static async Task AddConfigAsync(this WebApplication app)
    {
        var disableDatabaseInitialisation = app.Configuration.GetValue<bool>("Database:DisableInitialization", false);

        if (app.Environment.IsDevelopment() && !disableDatabaseInitialisation)
        {
            await app.InitialiseDatabaseAsync();

            app.MapOpenApi("/api/openapi/{documentName}.json");

            app.MapScalarApiReference(options =>
            {
                options
                .WithTitle("SKVIA Attendance — API Docs")
                .WithTheme(ScalarTheme.Laserwave)
                .AddDocument("v1", "Versión 1", routePattern: "/api/openapi/{documentName}.json")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

            });
        }

        app.UseExceptionHandler();

        app.Use(async (context, next) =>
        {
            context.Response.Headers["X-Content-Type-Options"] = "nosniff";
            context.Response.Headers["X-Frame-Options"] = "DENY";
            context.Response.Headers["Referrer-Policy"] = "no-referrer";
            context.Response.Headers["X-XSS-Protection"] = "0";
            context.Response.Headers["Cache-Control"] = "no-store";

            if (!context.Request.IsHttps && !app.Environment.IsDevelopment())
            {
                context.Response.Headers["Strict-Transport-Security"] = "max-age=31536000; includeSubDomains";
            }

            await next();
        });

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        // app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseRequestTimeouts();

        app.UseRateLimiter();

        app.UseOutputCache();

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<Skvia.Erp.Api.Common.Middleware.UserContextLoggingMiddleware>();

        app.Map("/", () => Results.Redirect("/scalar"));

        app.MapDefaultEndpoints();

        app.MapEndpoints(typeof(Program).Assembly);
    }
}

