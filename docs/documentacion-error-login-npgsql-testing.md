# Documentación del error: login con credenciales inválidas dispara PostgreSQL y devuelve 500

## Resumen

Se detectó un fallo en el flujo de autenticación en el backend que hace que una petición de login con credenciales inválidas termine con un `500 Internal Server Error` en vez de responder `401 Unauthorized`.

El problema no está en la validación de autenticación en sí, sino en la forma en que el sistema está instanciando el contexto de datos y en la lógica que continúa consultando base de datos incluso cuando el usuario o la contraseña son incorrectos.

---

## Síntoma observado

Cuando se ejecuta la petición:

```http
POST /api/v1/auth/login
Content-Type: application/json

{
  "userName": "noexiste",
  "password": "badpass"
}
```

la API responde con:

```http
HTTP/1.1 500 Internal Server Error
```

y el detalle del problema incluye algo como:

```text
The maximum number of retries (6) was exceeded while executing database operations with 'NpgsqlRetryingExecutionStrategy'.
```

El comportamiento correcto debería ser:

```http
HTTP/1.1 401 Unauthorized
```

con un problema de tipo autenticación, no un error interno.

---

## Evidencia reproducible

### Prueba de integración que falla

Archivo origen:

- tests/Skvia.Erp.Application.Tests/Integration/ApiAuthAndErrorTests.cs

Prueba relevante:

```csharp
[Fact]
public async Task Login_WhenBadCredentials_ReturnsUnauthorizedProblemDetails()
{
    var payload = new { userName = "noexiste", password = "badpass" };

    var response = await _client.PostAsJsonAsync("/api/v1/auth/login", payload);

    Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
}
```

Resultado real verificado:

- `Expected: Unauthorized`
- `Actual: InternalServerError`

---

## Causa raíz

### 1) El entorno de pruebas no estaba forzando un proveedor seguro para datos

El `DbContext` se registra en infraestructura con configuración basada en PostgreSQL:

- src/Skvia.Erp.Infrastructure/DependencyInjection.cs

Aunque el proyecto usa entorno `Testing`, el proveedor de base de datos puede seguir apuntando a `Npgsql` si la conexión está presente o si la condición del entorno no está correctamente detectada.

Esto provoca que la app intente abrir conexiones reales a PostgreSQL incluso en pruebas o ejecución con `ASPNETCORE_ENVIRONMENT=Testing`.

### 2) El flujo de login sigue consultando `Employees` incluso cuando la autenticación falla

El punto crítico está en:

- src/Skvia.Erp.Infrastructure/Services/IdentityUserAccountService.cs

El método `AuthenticateAsync` ejecuta esta lógica:

```csharp
var user = await userManager.FindByNameAsync(command.UserName);
if (user is null)
    return Error.Unauthorized("Credenciales Invalidas.");

var isPasswordValid = await userManager.CheckPasswordAsync(user, command.Password);
if (!isPasswordValid)
    return Error.Unauthorized("...");

var principal = await signInManager.CreateUserPrincipalAsync(user);

var userIdString = user.Id.ToString();
var employee = await dbContext.Employees
    .AsNoTracking()
    .FirstOrDefaultAsync(e => e.ApplicationUserId == userIdString, cancellationToken);
```

El problema aparece porque en una autenticación fallida la aplicación puede seguir yendo a base de datos para cargar información relacionada con el empleado, y si la conexión está rota o el proveedor no está preparado para pruebas, termina en la estrategia de reintentos de Npgsql.

### 3) El host de pruebas estaba compitiendo con configuración real de infraestructura

El host de tests también se configura en:

- tests/Skvia.Erp.Application.Tests/Integration/ApiAuthAndErrorTests.cs

y se intentó inflar un escenario con `WebApplicationFactory` usando `UseEnvironment("Testing")`, pero no se estaba garantizando que el `DbContext` usara `UseInMemoryDatabase` de forma consecuente.

Esto crea un entorno híbrido en el que la app se arranca como Testing pero la infraestructura continúa utilizando las dependencias reales de PostgreSQL.

---

## Impacto

Este problema afecta a:

- login de usuarios con credenciales inválidas,
- pruebas de integración de autenticación,
- estabilidad del entorno de pruebas,
- comportamiento de seguridad del backend, ya que un login inválido no debería ser un `500` sino un rechazo explícito con `401`.

---

## Corrección propuesta

### Paso 1: Forzar `InMemory` para entorno de testing

En el registro del `DbContext`, debe hacerse algo como:

```csharp
builder.Services.AddDbContext<ApplicationDbContext>((sp, opt) =>
{
    var interceptor = sp.GetRequiredService<ISaveChangesInterceptor>();
    var isTestingEnvironment = builder.Environment.IsEnvironment("Testing")
        || string.Equals(builder.Configuration["ASPNETCORE_ENVIRONMENT"], "Testing", StringComparison.OrdinalIgnoreCase)
        || string.Equals(builder.Configuration["DOTNET_ENVIRONMENT"], "Testing", StringComparison.OrdinalIgnoreCase)
        || builder.Configuration.GetValue<bool>("UseInMemoryDatabase");

    if (isTestingEnvironment || string.IsNullOrWhiteSpace(connectionString))
    {
        opt.UseInMemoryDatabase("skvia-testing-db");
    }
    else
    {
        opt.UseNpgsql(connectionString).AddInterceptors(interceptor);
    }

    opt.UseSnakeCaseNamingConvention();
    opt.AddInterceptors(interceptor);
});
```

Esto evita que la API de pruebas intente abrir PostgreSQL real.

### Paso 2: Desactivar la conexión PostgreSQL en el host de pruebas

En el `CustomWebApplicationFactory`, debe dejarse explícito:

```csharp
configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["Database:DisableInitialization"] = "true",
    ["UseInMemoryDatabase"] = "true",
    ["ConnectionStrings:skvia-erp-db"] = string.Empty
});
```

### Paso 3: Evitar consultas de BD en autenticación fallida

La validación debe retornar `Unauthorized` inmediatamente si:

- el usuario no existe,
- la contraseña no coincide,
- la cuenta está bloqueada,
- la cuenta está inactiva.

Se debe evitar que el sistema siga consultando `dbContext.Employees` cuando la autenticación ya ha fallado.

Una buena regla es:

- validar `user == null` antes de tocar base de datos adicional,
- validar contraseña antes de cualquier consulta relacionada con el empleado,
- ejecutar la comprobación de `employee` solo cuando la autenticación ha sido exitosa.

### Paso 4: Reforzar manejo de errores de auth como `401`, no como `500`

Cuando el problema real sea de credenciales o de disponibilidad de la BD durante una autenticación, debe devolver un `Error.Unauthorized(...)` en el servicio y no propagarse como excepción no controlada.

---

## Recomendación de validación

Ejecutar esta comprobación tras la corrección:

```bash
dotnet test tests/Skvia.Erp.Application.Tests/Skvia.Erp.Application.Tests.csproj --filter "ApiAuthAndErrorTests" --nologo
```

Debe devolver:

- `ProtectedEndpoint_WhenNoAuthentication_ReturnsUnauthorized` -> OK
- `Login_WhenBadCredentials_ReturnsUnauthorizedProblemDetails` -> OK
- `ProtectedEndpoint_WhenAuthenticated_ReturnsOk` -> OK
- `UnknownRoute_WhenRequested_ReturnsNotFound` -> OK

---

## Estado actual

Este problema todavía estaba en curso al momento de documentar la causa raíz. La última verificación evidencia que la corrección aún no está cerrada, pero la causa técnica quedó aislada y documentada.

La documentación sirve como base para implementar la corrección definitiva sin volver a introducir el mismo fallo.


