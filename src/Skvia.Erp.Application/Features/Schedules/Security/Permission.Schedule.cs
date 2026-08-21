using System.ComponentModel;
using Skvia.Erp.Application.Common.Attributes;

namespace Skvia.Erp.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos Plantillas de Horarios")]
    [Description("Establece los permisos para la gestión de plantillas de turnos y horarios de trabajo.")]
    public static class Schedule
    {
        [PermissionInfo("Ver Plantillas de Horario", "Permite ver la lista de plantillas de horario.")]
        public const string View = "Permissions.Schedules.View";

        [PermissionInfo("Crear Plantilla de Horario", "Permite crear nuevas plantillas de horario.")]
        public const string Create = "Permissions.Schedules.Create";

        [PermissionInfo("Editar Plantilla de Horario", "Permite modificar plantillas de horario existentes.")]
        public const string Update = "Permissions.Schedules.Update";

        [PermissionInfo("Eliminar Plantilla de Horario", "Permite eliminar plantillas de horario.")]
        public const string Delete = "Permissions.Schedules.Delete";
    }
}

