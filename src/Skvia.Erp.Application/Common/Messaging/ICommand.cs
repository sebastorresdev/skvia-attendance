using ErrorOr;

namespace Skvia.Erp.Application.Common.Messaging;

public interface ICommand;

public interface ICommand<TResponse> where TResponse : IErrorOr;


