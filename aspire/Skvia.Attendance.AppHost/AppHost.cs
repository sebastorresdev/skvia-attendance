IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("postgres-password", "postgres", secret: true);

// 🚀 1. Creamos el contenedor de PostgreSQL con su nombre técnico
var postgresServer = builder.AddPostgres("postgres", password: postgresPassword)
    .WithImage("postgres")
    .WithImageTag("16")
    .WithHostPort(5433)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin();

// 🚀 2. Declaramos la base de datos específica dentro del servidor Postgres
var database = postgresServer.AddDatabase("skvia-attendance-db");

// 🚀 3. Agregamos el proyecto WebApi y le inyectamos la referencia de la base de datos
var api = builder.AddProject<Projects.Skvia_Attendance_Api>("skvia-attendance-api")
    .WithReference(database)
    .WaitFor(database)
    .WithExternalHttpEndpoints()
    .WithUrlForEndpoint("https", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

// 🚀 4. Agregamos el Frontend Angular a la orquestación de Aspire
builder.AddNpmApp("skvia-attendance-frontend", "../../../skvia-attendance-frontend", scriptName: "start")
    .WithReference(api)
    .WaitFor(api)
    .WithHttpEndpoint(targetPort: 4200, name: "http")
    .WithExternalHttpEndpoints();

builder.Build().Run();
