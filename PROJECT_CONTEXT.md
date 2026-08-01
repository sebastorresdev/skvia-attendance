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