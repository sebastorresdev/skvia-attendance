using Skvia.Attendance.Infrastructure.Data;
using Skvia.Attendance.Api.Common.Extensions;

using Scalar.AspNetCore;


namespace Skvia.Attendance.Api;


public static class AppConfig
{
    public static async Task AddConfigAsync(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            await app.InitialiseDatabaseAsync();

            app.MapOpenApi("/api/{documentName}/openapi.json");

            app.MapScalarApiReference(options =>
            {
                options
                .WithTitle("BUBBA BAG — API Docs")
                .WithTheme(ScalarTheme.Saturn)
                .AddDocument("v1", "Versión 1", routePattern: "/api/v1/openapi.json")
                .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);

            });
        }

        app.UseExceptionHandler();

        app.UseHttpsRedirection();

        app.UseCors("AllowAll");

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        app.Map("/", () => Results.Redirect("/scalar"));

        app.MapDefaultEndpoints();

        app.MapEndpoints(typeof(Program).Assembly);
    }
}
