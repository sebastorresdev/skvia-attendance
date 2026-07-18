IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

// 🚀 1. Creamos el contenedor de PostgreSQL con su nombre técnico
var postgresServer = builder.AddPostgres("postgres")
    .WithImage("postgres")
    .WithImageTag("16")
    .WithHostPort(5433)
    .WithLifetime(ContainerLifetime.Persistent) // 💾 ¡NUEVO! Mantiene tus productos y usuarios vivos al reiniciar Aspire
    .WithPgAdmin();

// 🚀 2. Declaramos la base de datos específica dentro del servidor Postgres
var database = postgresServer.AddDatabase("bubba-db");

// 🚀 3. Agregamos el proyecto WebApi y le inyectamos la referencia de la base de datos
builder.AddProject<Projects.Skvia_Attendance_Api>("skvia-attendance-api")
    .WithReference(database)
    .WaitFor(database) // ⏳ ¡NUEVO! Espera a que el contenedor de Postgres esté 100% en verde antes de encender la API (evita errores de conexión al arrancar)
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("https", url => // 🪄 ¡NUEVO! Te genera un enlace directo con clic en el dashboard para probar tus endpoints
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar"; // Apunta a tu documentación interactiva
    }); ; // 🔥 Aspire amarra el ConnectionString automáticamente aquí

builder.Build().Run();
