using Skvia.Erp.Application.Common.Messaging;
using Microsoft.EntityFrameworkCore;
using ErrorOr;
using Skvia.Erp.Application.Common.Interfaces;

namespace Skvia.Erp.Application.Features.Workplaces.Commands.DeleteWorkplace;

public class DeleteWorkplaceCommandHandler(
    IApplicationDbContext dbContext) : ICommandHandler<DeleteWorkplaceCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(DeleteWorkplaceCommand command, CancellationToken cancellationToken)
    {
        var workplace = await dbContext.Workplaces.FindAsync(new object[] { command.Id }, cancellationToken);
        if (workplace is null)
        {
            return Error.NotFound("Workplace.NotFound", "Lugar de marcación no encontrado.");
        }

        dbContext.Workplaces.Remove(workplace);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success;
    }
}


