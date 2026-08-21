using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Security;

namespace Skvia.Erp.Application.Features.Justifications.Commands.ReviewJustification;

public class ReviewJustificationCommandHandler(
    IApplicationDbContext dbContext,
    ICurrentUserProvider currentUserProvider) : ICommandHandler<ReviewJustificationCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> HandleAsync(ReviewJustificationCommand command, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var reviewerUserId = currentUser.Id.ToString();

        var justification = await dbContext.Justifications
            .FirstOrDefaultAsync(j => j.Id == command.JustificationId, cancellationToken);

        if (justification is null)
            return Error.NotFound("Justification.NotFound", "Solicitud de justificación no encontrada.");

        if (command.Approve)
        {
            justification.Approve(reviewerUserId, command.Notes);
        }
        else
        {
            justification.Reject(reviewerUserId, command.Notes);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}

