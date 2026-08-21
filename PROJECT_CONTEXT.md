# 📋 PROJECT CONTEXT: Sistema de Control de Asistencia (Intranet)

## 🎯 Visión General del Proyecto
Sistema web interno de gestión y control de asistencia para personal, diseñado inicialmente como MVP para un caso de uso real (empresa/negocio) y preparado arquitectónicamente para escalar a futuro como un producto comercializable (SaaS / Multi-tenant).

---

## 🛠️ Stack Tecnológico
- **Backend:** C# (.NET 10) usando **Minimal APIs** y **ASP.NET Core Identity**.
- **Base de Datos:** **PostgreSQL** mediante **Entity Framework Core** (Migrations).
- **Frontend:** **Angular** (Standalone Components) + **NG-ZORRO** (Ant Design UI Library) + **TypeScript**.
- **Autenticación:** Tokens cifrados opacos / Cookies HttpOnly con un endpoint estandarizado `/api/auth/me` para la resolución de permisos, roles y datos del usuario en el cliente.

---

## 🏗️ Decisiones de Arquitectura y Negocio

### 1. Modelo de Negocio e Identidad
- **Sin Auto-Registro Público:** Es un sistema cerrado de gestión administrativa. La alta de empleados y asignación de credenciales (`UserName` y `Password` inicial) la realiza exclusivamente un Administrador / Recursos Humanos mediante `UserManager`.
- **Estructura Actual:** **Multi-sede / Multi-sucursal**. Un usuario o empleado pertenece a una Sede específica de la organización.
- **Preparado para Futuro Multi-tenant:** Diseñado de manera que en el futuro se pueda agregar la columna `EmpresaId` (Tenant) en las tablas principales y aplicar *Global Query Filters* en EF Core sin reescribir la aplicación.

### 2. Autenticación y Autorización
- El login recibe credenciales locales (`userName`, `password`), valida la cuenta activa y emite el token de sesión.
- El frontend (Angular) consulta `/api/auth/me` inmediatamente después de autenticarse para almacenar en un servicio global los datos del usuario, sus roles (`Admin`, `Supervisor`, `Empleado`) y su `SedeId`.
- Angular utiliza un `HttpInterceptor` para enviar el Bearer Token en cada solicitud y un `Functional Guard` para proteger las rutas de la intranet.

---

## 🗄️ Modelo de Datos Básico (PostgreSQL / EF Core)

1. **`Sedes`** (`Id`, `Nombre`, `Direccion`, `Estado`)
2. **`ApplicationUser`** (Hereda de `IdentityUser`):
   - `FirstName`, `LastName`, `Dni`, `IsActive`
   - `SedeId` (FK a `Sedes`)
3. **`Turnos`** (`Id`, `Nombre`, `HoraEntrada`, `HoraSalida`, `ToleranciaMinutos`, `SedeId`)
4. **`Marcaciones`** (`Id`, `UserId` [FK], `FechaHora`, `TipoMarcacion` [Entrada/Salida], `SedeId`, `Observacion`)

---

## 🚀 Guía de Estilo de Código para la IA
Cuando generes código o me ayudes a refactorizar dentro de este IDE, sigue estrictamente estas pautas:
- **Backend:** Usa sintaxis moderna de C# (Minimal APIs, `MapGroup`, instanciación de servicios con `inject` o DI de Minimal APIs, Types explícitos o fuertemente tipados).
- **Frontend:** Usa Angular moderno (Standalone Components, `inject()`, Signals para manejo de estado si aplica) y componentes nativos de **NG-ZORRO** (`nz-table`, `nz-form`, `nz-modal`, `nz-notification`).
- **Consultas EF Core:** Mantén las consultas optimizadas (`AsNoTracking()` para lecturas, filtrado directo por `SedeId`).

---

## ♻️ Refactorización de Entidades y Objetos de Valor (Realizado por Gemini)

Se ha llevado a cabo una refactorización significativa en la entidad `Employee` para mejorar la encapsulación, la validación y la consistencia del dominio mediante la introducción de objetos de valor.

**Cambios Clave:**

1.  **Entidad `Employee` (Dominio):**
    *   Se reemplazaron las propiedades `DocumentType` (int) y `DocumentNumber` (string) por el objeto de valor `DocumentIdentifier`.
    *   Se reemplazaron las propiedades `Email` (string) y `Phone` (string) por los objetos de valor `Email` y `Phone` respectivamente.
    *   Los métodos `Create` y `Update` de la entidad `Employee` ahora aceptan y utilizan estos nuevos objetos de valor, delegando la lógica de validación intrínseca a los mismos.

2.  **Objetos de Valor Creados (Dominio):**
    *   `DocumentIdentifier` (record struct): Encapsula el tipo y número de documento, con validación de longitud.
    *   `Email` (record struct): Encapsula la dirección de correo electrónico, con validación de formato y longitud.
    *   `Phone` (record struct): Encapsula el número de teléfono, con validación de formato y longitud.

3.  **Capa de Aplicación (`Skvia.Erp.Application`):**
    *   **Comandos (`CreateEmployeeCommand`, `UpdateEmployeeCommand`):** Se actualizaron para usar el `enum DocumentType` del dominio y para aceptar `string?` para `Email` y `Phone`. La conversión a los objetos de valor se realiza en los `CommandHandlers`.
    *   **Manejadores de Comandos (`CreateEmployeeCommandHandler`, `UpdateEmployeeCommandHandler`):** Se modificaron para construir los objetos de valor `DocumentIdentifier`, `Email` y `Phone` a partir de los datos del comando antes de interactuar con la entidad `Employee`. Se actualizó la lógica de verificación de unicidad para `DocumentIdentifier`.
    *   **Validadores de Comandos (`CreateEmployeeCommandValidator`, `UpdateEmployeeCommandValidator`):** Se ajustaron para delegar las validaciones de longitud y formato específicas a los objetos de valor, manteniendo las validaciones de presencia y formato general a nivel de comando.
    *   **DTOs (`EmployeeResponse`, `EmployeeDetailResponse`):** Se actualizaron para incluir las propiedades `DocumentType`, `DocumentNumber`, `Email` y `Phone` de los objetos de valor, exponiéndolos como tipos primitivos (ej. `string?` para Email/Phone) para simplificar el consumo por parte del cliente.
    *   **Manejadores de Consultas (`GetEmployeesQueryHandler`, `GetEmployeeByIdQueryHandler`):** Se modificaron las proyecciones para extraer correctamente los valores de los objetos de valor (`DocumentIdentifier.Type`, `DocumentIdentifier.Number`, `Email.Value`, `Phone.Value`) al construir los DTOs de respuesta.

4.  **Capa de Infraestructura (`Skvia.Erp.Infrastructure`):**
    *   **Configuración de Entidad `Employee` (`EmployeeConfiguration.cs`):**
        *   `DocumentIdentifier` se configuró como una entidad poseída (`builder.OwnsOne`), mapeando `Type` y `Number` a columnas separadas (`DocumentType` y `DocumentNumber`) en la tabla `employees`. Se estableció un índice único combinado para `DocumentType` y `DocumentNumber`.
        *   `Email` y `Phone` se configuraron con `ValueConverter` para mapear sus propiedades `Value` a columnas de tipo `string` en la base de datos, manejando la nulabilidad y las longitudes máximas.

Estos cambios mejoran la robustez del modelo de dominio, centralizan la lógica de validación y preparan la aplicación para un manejo de errores más consistente.

