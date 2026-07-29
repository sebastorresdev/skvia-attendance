namespace Skvia.Attendance.Application.Common.Security.Permissions;

public record PermissionCatalogItemDto(
    string Key,
    string Display,
    string Description
);

public record PermissionCatalogGroupDto(
    string Group,
    string GroupDescription,
    List<PermissionCatalogItemDto> Permissions
);
