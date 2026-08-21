using System.ComponentModel;
using Skvia.Erp.Application.Common.Attributes;

namespace Skvia.Erp.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos Horarios de Empleados")]
    [Description("Establece los permisos para la asignación y generación de horarios de empleados.")]
    public static class EmployeeSchedule
    {
        [PermissionInfo("Ver Horarios de Empleados", "Permite visualizar la asignación de horarios de empleados.")]
        public const string View = "Permissions.EmployeeSchedules.View";

        [PermissionInfo("Asignar Horarios", "Permite asignar u horarizar turnos a empleados.")]
        public const string Assign = "Permissions.EmployeeSchedules.Assign";

        [PermissionInfo("Generar Horarios", "Permite generar horarios automáticos para empleados.")]
        public const string Generate = "Permissions.EmployeeSchedules.Generate";
    }
}

