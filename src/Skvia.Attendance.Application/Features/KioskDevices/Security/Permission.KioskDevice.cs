using System.ComponentModel;
using Skvia.Attendance.Application.Common.Attributes;

namespace Skvia.Attendance.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Dispositivos Kiosko")]
    [Description("Permisos relacionados a la administración de kioskos")]
    public static class KioskDevices
    {
        [PermissionInfo("Ver Dispositivos", "Permite listar y ver los kioskos vinculados.")]
        public const string View = "Permissions.KioskDevices.View";

        [PermissionInfo("Vincular Dispositivo", "Permite autorizar un nuevo dispositivo kiosko.")]
        public const string Link = "Permissions.KioskDevices.Link";

        [PermissionInfo("Revocar Dispositivo", "Permite revocar el acceso de un kiosko existente.")]
        public const string Revoke = "Permissions.KioskDevices.Revoke";
    }
}
