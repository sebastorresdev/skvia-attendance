using Scalar.AspNetCore;

using Skvia.Attendance.Infrastructure;
using Skvia.Attendance.Infrastructure.Data;
using Skvia.Attendance.Infrastructure.Identity;
using Skvia.Attendance.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddInfrastructureServices()
    .AddWebServices();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
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

app.MapIdentityApi<ApplicationUser>();

app.Map("/", () => Results.Redirect("/scalar"));

app.MapDefaultEndpoints();

app.Run();
