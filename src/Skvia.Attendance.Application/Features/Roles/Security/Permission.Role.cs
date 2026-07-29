using System.ComponentModel;

using Skvia.Attendance.Application.Common.Attributes;

namespace Skvia.Attendance.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos de Roles")]
    [Description("Define los permisos para la administración de roles")]
    public static class Role
    {
        [PermissionInfo("Ver Roles", "Permite ver la lista de roles")]
        public const string View = "Permissions.Roles.View";

        [PermissionInfo("Crear Rol", "Permite crear nuevos roles")]
        public const string Create = "Permissions.Roles.Create";

        [PermissionInfo("Editar Rol", "Permite editar roles existentes")]
        public const string Edit = "Permissions.Roles.Edit";

        [PermissionInfo("Eliminar Rol", "Permite eliminar roles")]
        public const string Delete = "Permissions.Roles.Delete";
    }
}
