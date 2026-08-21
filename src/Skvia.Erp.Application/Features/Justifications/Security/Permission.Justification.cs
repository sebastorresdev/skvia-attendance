using System.ComponentModel;
using Skvia.Erp.Application.Common.Attributes;

namespace Skvia.Erp.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos Justificaciones")]
    [Description("Establece los permisos para la gestión de justificaciones de tardanza e inasistencia.")]
    public static class Justification
    {
        [PermissionInfo("Ver Justificaciones", "Permite visualizar las solicitudes de justificación.")]
        public const string View = "Permissions.Justifications.View";

        [PermissionInfo("Crear Justificación", "Permite solicitar o registrar una justificación.")]
        public const string Create = "Permissions.Justifications.Create";

        [PermissionInfo("Aprobar Justificación", "Permite aprobar o rechazar justificaciones.")]
        public const string Approve = "Permissions.Justifications.Approve";
    }
}

