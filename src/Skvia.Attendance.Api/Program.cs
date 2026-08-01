using Serilog;
using Serilog.Events;

using Skvia.Attendance.Api;
using Skvia.Attendance.Application;
using Skvia.Attendance.Infrastructure;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .MinimumLevel.Override("System", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "Skvia.Attendance.Api")
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
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
