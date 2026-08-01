namespace Skvia.Attendance.Application.Common.Security.Permissions;

public record PermissionCatalogItemResponse(
    string Key,
    string Display,
    string Description
);

public record PermissionCatalogGroupResponse(
    string Group,
    string GroupDescription,
    List<PermissionCatalogItemResponse> Permissions
);
