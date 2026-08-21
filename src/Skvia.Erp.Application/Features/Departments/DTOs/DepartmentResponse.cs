namespace Skvia.Erp.Application.Features.Departments.DTOs;

public record DepartmentResponse(
    Guid Id,
    string Name,
    string? Description);

