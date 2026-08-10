using System.ComponentModel;
using Skvia.Attendance.Application.Common.Attributes;

namespace Skvia.Attendance.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Sistema")]
    [Description("Permisos generales del sistema")]
    public static class System
    {
        [PermissionInfo("Acceso Básico", "Permite el acceso básico al sistema")]
        public const string Access = "Permissions.System.Access";
    }
}
