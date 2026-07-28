using Serilog;

using Skvia.Attendance.Api;
using Skvia.Attendance.Application;
using Skvia.Attendance.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando el servidor web de la API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddSerilog((services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(builder.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder
        .AddInfrastructureServices()
        .AddApplicationServices()
        .AddWebServices();

    var app = builder.Build();

    await app.AddConfigAsync();

    app.Run();

}
catch (Exception ex)
{
    Log.Fatal(ex, "Error crítico al mapear endpoints o iniciar la aplicación: {Message}", ex.Message);
    throw;
}
finally
{
    Log.CloseAndFlush();
}

return 0;
