using System.ComponentModel;
using Skvia.Erp.Application.Common.Attributes;

namespace Skvia.Erp.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos Dashboard")]
    [Description("Establece los permisos para el panel de control y alertas.")]
    public static class Dashboard
    {
        [PermissionInfo("Ver Dashboard", "Permite visualizar el panel de control, resumenes y alertas de asistencia.")]
        public const string View = "Permissions.Dashboard.View";
    }
}

