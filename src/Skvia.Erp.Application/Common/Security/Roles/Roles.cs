namespace Skvia.Erp.Application.Common.Security.Roles;

/// <summary>
/// Roles funcionales predefinidos del sistema de Recursos Humanos.
/// </summary>
public static class Roles
{
    /// <summary> Rol de Desarrollador / Administrador técnico del sistema con acceso total a roles, permisos y configuraciones. </summary>
    public const string SuperAdmin = "SuperAdmin";

    /// <summary> Rol de Administrador de Recursos Humanos para la gestión de empleados, departamentos, sedes y horarios. </summary>
    public const string HRAdmin = "HRAdmin";

    /// <summary> Rol de Supervisor o Jefe de Área para la gestión de su equipo y aprobación de solicitudes/justificaciones. </summary>
    public const string Supervisor = "Supervisor";

    /// <summary> Rol de Empleado para autoservicio, consulta de asistencias y solicitud de justificaciones. </summary>
    public const string Employee = "Empleado";

    /// <summary> Rol de Dispositivo Kiosco para terminales de marcación en sedes. </summary>
    public const string KioskDevice = "KioskDevice";

    /// <summary> Alias de compatibilidad para el administrador principal. </summary>
    public const string Administrator = SuperAdmin;
}


