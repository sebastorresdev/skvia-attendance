namespace Skvia.Erp.Application.Common.DTOs;

public record PermissionGroupResponse(
    string Group,
    string GroupDescription,
    List<PermissionItemResponse> Permissions
);

