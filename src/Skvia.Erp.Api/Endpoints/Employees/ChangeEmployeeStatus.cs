using Skvia.Erp.Api.Common.Extensions;
using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Skvia.Erp.Api.Models;
using Skvia.Erp.Application.Features.Employees.Commands.ChangeEmployeeStatus;
using Skvia.Erp.Domain.Employees;

namespace Skvia.Erp.Api.Endpoints.Employees;

public sealed class ChangeEmployeeStatus : IEndpoint
{
    public static void Map(RouteGroupBuilder group)
        => group.MapPut("/{id:guid}/status", Handle)
            .WithName(nameof(ChangeEmployeeStatus))
            .WithSummary("Cambiar estado de empleado")
            .WithDescription("Cambia el estado de un empleado (Activo, Inactivo, etc).")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ApiProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ApiProblemDetails>(StatusCodes.Status404NotFound);

    private static async Task<IResult> Handle(
        Guid id,
        [FromBody] ChangeStatusRequest request,
        ICommandHandler<ChangeEmployeeStatusCommand, ErrorOr<Success>> handler,
        CancellationToken cancellationToken)
    {
        var command = new ChangeEmployeeStatusCommand(id, request.Status);
        
        var result = await handler.HandleAsync(command, cancellationToken);
        
        return result.Match(
            _ => TypedResults.NoContent(),
            errors => errors.ToProblem());
    }
}

public record ChangeStatusRequest(EmployeeStatus Status);



