namespace Skvia.Attendance.Application.Common.Messaging;

public interface IQuery<out TResponse> where TResponse : IErrorOr;
