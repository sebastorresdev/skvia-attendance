using System.ComponentModel;

using Skvia.Attendance.Application.Common.Attributes;

namespace Skvia.Attendance.Application.Common.Security.Permissions;

public static partial class Permissions
{
    [DisplayName("Permisos de Usuarios")]
    [Description("Define los permisos para la administración de usuarios")]
    public static class Users
    {
        [PermissionInfo("Ver Usuarios", "Permite ver la lista de usuarios")]
        public const string View = "Permissions.Users.View";

        [PermissionInfo("Crear Usuario", "Permite crear nuevos usuarios")]
        public const string Create = "Permissions.Users.Create";

        [PermissionInfo("Editar Usuario", "Permite editar usuarios existentes")]
        public const string Edit = "Permissions.Users.Edit";

        [PermissionInfo("Eliminar Usuario", "Permite eliminar usuarios")]
        public const string Delete = "Permissions.Users.Delete";
    }
}
