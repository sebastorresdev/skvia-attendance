using ErrorOr;

namespace Skvia.Erp.Application.Common.Messaging;

public interface IQuery<out TResponse> where TResponse : IErrorOr;


