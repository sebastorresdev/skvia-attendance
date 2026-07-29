using System.ComponentModel;
using System.Reflection;

using Skvia.Attendance.Application.Common.Attributes;
using Skvia.Attendance.Application.Features.Permissions.DTOs;
namespace Skvia.Attendance.Application.Features.Permissions.Queries.GetPermissions;

public class GetPermissionsQueryHandler : IQueryHandler<GetPermissionsQuery, ErrorOr<List<PermissionGroupDto>>>
{
    public async Task<ErrorOr<List<PermissionGroupDto>>> HandleAsync(
        GetPermissionsQuery query,
        CancellationToken cancellationToken)
    {
        var permissions = new List<PermissionGroupDto>();

        var groups = typeof(Common.Security.Permissions.Permissions)
            .GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

        foreach (var group in groups)
        {
            var groupDisplay = group.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName
                               ?? group.Name;

            var groupDescription = group.GetCustomAttribute<DescriptionAttribute>()?.Description
                                  ?? string.Empty;

            var permissionsItem = new List<PermissionItemDto>();

            var fields = group.GetFields(BindingFlags.Public | BindingFlags.Static);

            foreach (var field in fields)
            {
                var attr = field.GetCustomAttribute<PermissionInfoAttribute>();
                if (attr == null)
                    continue;

                permissionsItem.Add(new PermissionItemDto
                (
                    Key: field.GetValue(null)?.ToString() ?? "",
                    Display: attr.Display,
                    Description: attr.Description
                ));
            }

            permissions.Add(new PermissionGroupDto
            (
               Group: groupDisplay,
               GroupDescription: groupDescription,
               Permissions: permissionsItem
            ));
        }

        return permissions;
    }
}
