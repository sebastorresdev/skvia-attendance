using System.ComponentModel;
using Skvia.Erp.Application.Common.Attributes;

namespace Skvia.Erp.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos Asistencias")]
    [Description("Establece los permisos para las operaciones de marcado y registro de asistencia.")]
    public static class Attendance
    {
        [PermissionInfo("Ver Asistencias", "Permite visualizar el registro de asistencias.")]
        public const string View = "Permissions.Attendances.View";

        [PermissionInfo("Registrar Asistencia", "Permite registrar la asistencia manual o mediante dispositivo.")]
        public const string Register = "Permissions.Attendances.Register";

        [PermissionInfo("Exportar Asistencia", "Permite exportar los reportes de asistencia.")]
        public const string Export = "Permissions.Attendances.Export";
    }
}

