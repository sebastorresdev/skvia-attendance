using System.ComponentModel;
using Skvia.Erp.Application.Common.Attributes;

namespace Skvia.Erp.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos de Usuarios")]
    [Description("Define los permisos para la administración de usuarios")]
    public static class User
    {
        [PermissionInfo("Ver Usuarios", "Permite ver la lista de usuarios")]
        public const string View = "Permissions.Users.View";

        [PermissionInfo("Crear Usuario", "Permite crear nuevos usuarios")]
        public const string Create = "Permissions.Users.Create";

        [PermissionInfo("Editar Usuario", "Permite editar usuarios existentes")]
        public const string Edit = "Permissions.Users.Edit";
        public const string Update = Edit;

        [PermissionInfo("Eliminar Usuario", "Permite eliminar usuarios")]
        public const string Delete = "Permissions.Users.Delete";
    }
}

