using System.ComponentModel;
using Skvia.Erp.Application.Common.Attributes;

namespace Skvia.Erp.Application.Common.Security;

public static partial class Permission
{
    [DisplayName("Permisos Departamentos")]
    [Description("Establece los permisos para la gestión de departamentos organizacionales.")]
    public static class Department
    {
        [PermissionInfo("Ver Departamento", "Permite ver la lista de departamentos.")]
        public const string View = "Permissions.Departments.View";

        [PermissionInfo("Crear Departamento", "Permite registrar un nuevo departamento.")]
        public const string Create = "Permissions.Departments.Create";

        [PermissionInfo("Editar Departamento", "Permite actualizar un departamento.")]
        public const string Update = "Permissions.Departments.Update";

        [PermissionInfo("Eliminar Departamento", "Permite eliminar un departamento.")]
        public const string Delete = "Permissions.Departments.Delete";
    }
}

