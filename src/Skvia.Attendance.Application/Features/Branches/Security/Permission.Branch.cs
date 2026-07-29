using System.ComponentModel;

using Skvia.Attendance.Application.Common.Attributes;

namespace Skvia.Attendance.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos Sedes")]
    [Description("Establece los permisos para las operaciones con sedes")]
    public static class Branch
    {
        [PermissionInfo("Crear Sucursal", "Permite crear una sucursal")]
        public const string Create = "Permissions.Branches.Create";

        [PermissionInfo("Actualizar Sucursal", "Permite actualizar una sucursal")]
        public const string Update = "Permissions.Branches.Update";

        [PermissionInfo("Eliminar Sucursal", "Permite eliminar una sucursal")]
        public const string Delete = "Permissions.Branches.Delete";

        [PermissionInfo("Ver Sucursales", "Permite ver la lista de sucursales")]
        public const string View = "Permissions.Branches.View";
    }
}
