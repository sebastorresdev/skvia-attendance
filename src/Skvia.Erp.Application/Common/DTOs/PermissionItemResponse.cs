namespace Skvia.Erp.Application.Common.DTOs;

public record PermissionItemResponse(
    string Key,
    string Display,
    string Description,
    bool Granted,
    string? Source
);

