# Plan de mejoras para arquitectura y calidad

Este documento recoge las correcciones que haría para mejorar los puntos 1, 2, 4 y 5 de la propuesta anterior, aplicadas al proyecto actual de Skvia Attendance.

## Objetivo

Hacer que el proyecto sea más limpio, más mantenible, más testeable y más alineado con una arquitectura limpia, sin perder funcionalidad ni rapidez de desarrollo.

---

## 1. Evitar que Application dependa de Infrastructure

### Problema actual

La capa Application está empezando a depender de detalles concretos de Infrastructure, especialmente en los handlers de usuarios y autenticación. Esto rompe la separación de capas y dificulta probar la lógica de negocio sin depender de la implementación concreta de Identity o EF Core.

### Correcciones propuestas

1. Crear interfaces de aplicación para los servicios de infraestructura que la lógica de negocio necesita.
   - Ejemplo: `IIdentityService` en Application.
   - Métodos sugeridos:
     - `CreateUserAsync(...)`
     - `LoginAsync(...)`
     - `ResetPasswordAsync(...)`
     - `UpdateUserAsync(...)`

2. Mover la implementación concreta a Infrastructure.
   - `UserManager<ApplicationUser>` y otros detalles de ASP.NET Identity deberían quedar encapsulados en Infrastructure.
   - Los handlers de Application deberían trabajar con interfaces, no con `UserManager` directamente.

3. Eliminar referencias de Application a namespaces de Infrastructure.
   - Ejemplo actual: `Skvia.Erp.Infrastructure.Identity.Domain` usado en handlers de Application.
   - Esto debe corregirse y mover la definición de errores relacionados a un namespace correcto de Domain, por ejemplo:
     - `Skvia.Erp.Domain.Identity`

4. Mantener `IApplicationDbContext` como la única dependencia de persistencia visible desde Application.
   - Eso ya está bien orientado, pero conviene extender el patrón a otros servicios como identidad, archivos, correos o permisos.

### Archivos que probablemente se modificarían

- `src/Skvia.Erp.Application/Features/Users/Commands/CreateUser/CreateUserCommandHandler.cs`
- `src/Skvia.Erp.Application/Features/Users/Commands/ResetPassword/ResetPasswordCommandHandler.cs`
- `src/Skvia.Erp.Application/Features/Users/Commands/UpdateUser/UpdateUserCommandHandler.cs`
- `src/Skvia.Erp.Application/Features/Users/Queries/GetUserById/GetUserByIdQueryHandler.cs`
- `src/Skvia.Erp.Domain/Identity/UserErrors.cs`

### Resultado esperado

- La lógica de negocio ya no depende de los detalles técnicos de Identity.
- Los handlers se vuelven más simples y fáciles de probar.
- La arquitectura queda más limpia y preparada para crecer.

---

## 2. Hacer la capa Domain más pura

### Problema actual

La capa Domain contiene lógica de negocio, pero todavía mezcla decisiones técnicas con reglas de negocio. Ejemplos claros:

- `Attendance.CreateCheckIn(...)` usa directamente `DateTimeOffset.UtcNow` y `TimeZoneInfo.FindSystemTimeZoneById(...)`.
- La lógica de asistencia depende del reloj real del sistema y del entorno de ejecución.

### Correcciones propuestas

1. Introducir abstracciones para el tiempo y la zona horaria.
   - Crear interfaces como:
     - `IClock`
     - `ITimeZoneProvider`
   - Estas interfaces deberían vivir en Domain o en una capa de abstractions compartida del dominio.

2. Mover la lógica sensible al tiempo a un servicio de dominio o a un componente específico.
   - En lugar de que la entidad `Attendance` dependa de la implementación real del sistema, se puede encapsular el cálculo en un servicio como:
     - `IAttendanceDomainService`
   - Este servicio recibiría el reloj y el proveedor de zona horaria como dependencias.

3. Reducir el uso de `DateTimeOffset.UtcNow` y `TimeZoneInfo` dentro del modelo de dominio.
   - El dominio debería expresar reglas de negocio, no depender del runtime.
   - Esto además facilita las pruebas unitarias porque se puede simular el tiempo.

4. Mantener las entidades del domain enfocadas en estado y comportamiento, no en infraestructura.
   - Por ejemplo, `Attendance` debería centrarse en el estado de la asistencia y sus reglas internas, no en cómo se obtiene el tiempo del sistema.

### Archivos que probablemente se modificarían

- `src/Skvia.Erp.Domain/Attendances/Attendance.cs`
- `src/Skvia.Erp.Domain/EmployeeSchedules/EmployeeSchedule.cs`
- `src/Skvia.Erp.Domain/Common/` (nueva ubicación de abstracciones compartidas)
- `src/Skvia.Erp.Infrastructure/` (implementación concreta de `IClock` y `ITimeZoneProvider`)

### Resultado esperado

- El dominio será más puro y más fácil de probar.
- Las reglas de negocio serán independientes del entorno.
- Se podrá probar el comportamiento de asistencia sin depender del reloj real.

---

## 4. Eliminar credenciales y datos de seed hardcodeados

### Problema actual

El seeding inicial del sistema está haciendo cosas muy prácticas pero poco seguras:

- Se crea un usuario administrador con contraseña fija.
- El password aparece directamente en el código.
- El sistema ejecuta borrado y recreación de la base de datos al iniciar.

### Correcciones propuestas

1. Quitar valores hardcodeados del código.
   - En lugar de:
     - `"Password123*"`
   - usar configuración externa.

2. Configurar el seed mediante opciones de configuración.
   - Ejemplo de estructura:

```json
{
  "Seed": {
    "Enabled": true,
    "Admin": {
      "UserName": "admin",
      "Email": "admin@skvia.pe",
      "Password": "***"
    }
  }
}
```

3. Usar usuarios secretos o variables de entorno en desarrollo.
   - En producción, no dejar la contraseña en código ni en archivos de configuración versionados.
   - Para desarrollo se puede usar `dotnet user-secrets` o variables de entorno.

4. Evitar `EnsureDeletedAsync()` en el arranque.
   - Hoy el inicializador borra y recrea la base de datos.
   - Eso es útil para demos, pero no es una estrategia adecuada para un proyecto que va a evolucionar.
   - Lo ideal es:
     - usar migraciones de EF Core,
     - aplicar migraciones durante el arranque o en un proceso de despliegue,
     - y dejar el seeding controlado y opcional.

5. Hacer que el seeding sea idempotente.
   - Si el usuario ya existe, no volver a insertarlo.
   - Si el rol ya existe, no volver a crearlo.

### Archivos que probablemente se modificarían

- `src/Skvia.Erp.Infrastructure/Data/ApplicationDbContextInitialiser.cs`
- `src/Skvia.Erp.Api/appsettings.json`
- `src/Skvia.Erp.Api/appsettings.Development.json`
- `src/Skvia.Erp.Api/Program.cs` o el lugar donde se invoca la inicialización

### Resultado esperado

- El proyecto será más seguro.
- El arranque de la base de datos será más controlado.
- El comportamiento será más predecible y menos riesgoso en entornos reales.

---

## 5. Añadir tests de forma seria

### Problema actual

El proyecto tiene una estructura muy buena, pero aún no muestra una base sólida de pruebas. Eso puede hacer que el crecimiento del proyecto se vuelva frágil.

### Correcciones propuestas

1. Crear un proyecto de pruebas unitarias para Domain.
   - Ejemplos de pruebas:
     - `Branch.Create` con datos válidos.
     - `Branch.Create` con nombres o códigos demasiado largos.
     - `Employee.Create` con datos correctos.
     - `Attendance.CreateCheckIn` calcula tardanza correctamente.
     - `EmployeeSchedule` valida horas para días laborables y no laborables.

2. Crear un proyecto de pruebas para Application.
   - Probar handlers y validadores.
   - Ejemplos:
     - `CreateUserCommandHandler` crea correctamente un usuario válido.
     - `CreateUserCommandHandler` devuelve error si el usuario ya existe.
     - `LoginCommandHandler` devuelve error con credenciales inválidas.

3. Añadir pruebas de integración para la API.
   - Probar endpoints críticos como:
     - `POST /api/auth/login`
     - `GET /api/branches`
     - `POST /api/users`
   - Estas pruebas deberían usar una base de datos temporal o un contenedor PostgreSQL de prueba.

4. Utilizar herramientas adecuadas.
   - Recomendación:
     - xUnit
     - FluentAssertions
     - Moq
     - Testcontainers (para PostgreSQL)

5. Definir una estrategia clara de cobertura.
   - No basta con tener tests; hay que priorizar los casos de negocio más sensibles.
   - En este proyecto, los más importantes serían:
     - autenticación,
     - creación y actualización de usuarios,
     - lógica de asistencia,
     - permisos y roles.

### Estructura sugerida de test projects

- `tests/Skvia.Erp.Domain.Tests`
- `tests/Skvia.Erp.Application.Tests`
- `tests/Skvia.Erp.Api.Tests`

### Resultado esperado

- Menos regresiones al modificar el sistema.
- Mayor confianza al introducir nuevas funcionalidades.
- Mejor base para evolucionar el proyecto sin miedo.

---

## Orden de implementación recomendado

1. Introducir abstracciones para identidad y servicios de aplicación.
2. Separar el tiempo y la zona horaria del dominio.
3. Reemplazar el seeding hardcodeado por configuración segura.
4. Cambiar la inicialización de base de datos a una estrategia más madura.
5. Crear proyectos de pruebas y cubrir los casos críticos primero.

---

## Criterios de aceptación

El proyecto se considerará mejorado cuando:

- Application ya no dependa directamente de detalles concretos de Infrastructure.
- El dominio pueda probarse sin depender del reloj real del sistema.
- El seeding no tenga credenciales hardcodeadas.
- Existan tests unitarios y de integración para los flujos más críticos.

---

## Resumen corto

Si aplico estas mejoras, el proyecto pasará de ser un backend funcional a un backend mucho más sólido, limpio y preparado para crecer.

