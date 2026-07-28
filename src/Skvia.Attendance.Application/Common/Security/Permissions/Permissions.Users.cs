using System.ComponentModel;

namespace Skvia.Attendance.Application.Common.Security.Permissions;

public static partial class Permissions
{
    public static class Users
    {
        [Description("Ver Usuarios")]
        public const string View = "Permissions.Users.View";

        [Description("Crear Usuario")]
        public const string Create = "Permissions.Users.Create";

        [Description("Editar Usuario")]
        public const string Edit = "Permissions.Users.Edit";

        [Description("Eliminar Usuario")]
        public const string Delete = "Permissions.Users.Delete";
    }
}
