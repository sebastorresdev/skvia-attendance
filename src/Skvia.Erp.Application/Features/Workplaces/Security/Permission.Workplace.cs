using System.ComponentModel;
using Skvia.Erp.Application.Common.Attributes;

namespace Skvia.Erp.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos Lugares de Marcación")]
    [Description("Establece los permisos para las operaciones con lugares de marcación")]
    public static class Workplace
    {
        [PermissionInfo("Crear Lugar de Marcación", "Permite crear un lugar de marcación")]
        public const string Create = "Permissions.Workplaces.Create";

        [PermissionInfo("Actualizar Lugar de Marcación", "Permite actualizar un lugar de marcación")]
        public const string Update = "Permissions.Workplaces.Update";

        [PermissionInfo("Eliminar Lugar de Marcación", "Permite eliminar un lugar de marcación")]
        public const string Delete = "Permissions.Workplaces.Delete";

        [PermissionInfo("Ver Lugares de Marcación", "Permite ver la lista de lugares de marcación")]
        public const string View = "Permissions.Workplaces.View";
    }
}

