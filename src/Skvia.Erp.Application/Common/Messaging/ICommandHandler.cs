using ErrorOr;

namespace Skvia.Erp.Application.Common.Messaging;

public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : IErrorOr
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
}


