namespace Skvia.Attendance.Application.Common.Messaging;

public interface ICommand;

public interface ICommand<TResponse> where TResponse : IErrorOr;
