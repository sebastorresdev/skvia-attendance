using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Domain.Justifications;

namespace Skvia.Erp.Application.Features.Justifications.Commands.CreateJustification;

public class CreateJustificationCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<CreateJustificationCommand, ErrorOr<Guid>>
{
    public async Task<ErrorOr<Guid>> HandleAsync(CreateJustificationCommand command, CancellationToken cancellationToken)
    {
        var employeeExists = await dbContext.Employees.AnyAsync(e => e.Id == command.EmployeeId, cancellationToken);
        if (!employeeExists)
            return Error.NotFound("Employee.NotFound", "Empleado no encontrado.");

        var existingJustification = await dbContext.Justifications
            .AnyAsync(j => j.EmployeeId == command.EmployeeId && j.Date == command.Date && j.Type == command.Type, cancellationToken);

        if (existingJustification)
            return Error.Conflict("Justification.AlreadyExists", "Ya existe una solicitud de justificación para este empleado en la fecha seleccionada.");

        var justification = Justification.Create(
            command.EmployeeId,
            command.Date,
            command.Type,
            command.Reason,
            command.DocumentUrl);

        dbContext.Justifications.Add(justification);
        await dbContext.SaveChangesAsync(cancellationToken);

        return justification.Id;
    }
}

