using System.ComponentModel;
using System.Reflection;

using Skvia.Attendance.Application.Common.Attributes;

namespace Skvia.Attendance.Application.Common.Security.Permissions;

public static class PermissionCatalog
{
    public static List<PermissionCatalogGroupResponse> GetAll()
    {
        var permissions = new List<PermissionCatalogGroupResponse>();

        var groups = typeof(Permission).GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var group in groups)
        {
            var groupDisplay = group.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                               ?? group.Name;

            var groupDescription = group.GetCustomAttribute<DescriptionAttribute>()?.Description
                                  ?? string.Empty;

            var permissionsItem = new List<PermissionCatalogItemResponse>();

            var fields = group.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<PermissionInfoAttribute>();
                if (attr == null)
                    continue;

                permissionsItem.Add(new PermissionCatalogItemResponse(
                    Key: field.GetValue(null)?.ToString() ?? "",
                    Display: attr.Display,
                    Description: attr.Description
                ));
            }

            permissions.Add(new PermissionCatalogGroupResponse(
                Group: groupDisplay,
                GroupDescription: groupDescription,
                Permissions: permissionsItem
            ));
        }

        return permissions;
    }
}
