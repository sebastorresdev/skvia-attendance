using System.ComponentModel;

using Skvia.Attendance.Application.Common.Attributes;

namespace Skvia.Attendance.Application.Common.Security.Permissions;


public partial class Permissions
{
    [DisplayName("Permisos Empleados")]
    [Description("Establece los permisos para las operaciones con empleados.")]
    public static class Employees
    {
        [PermissionInfo("Ver Empleado", "Permite ver la lista de empleados.")]
        public const string View = "Permissions.Employees.View";

        [PermissionInfo("Crear Empleado", "Permite crear un empleado.")]
        public const string Create = "Permissions.Employees.Create";

        [PermissionInfo("Editar Empleado", "Permite editar un empleado.")]
        public const string Update = "Permissions.Employees.Update";

        [PermissionInfo("Eliminar Empleado", "Permite eliminar un empleado.")]
        public const string Delete = "Permissions.Employees.Delete";
    }
}
