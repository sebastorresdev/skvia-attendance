using Serilog.Context;
using Skvia.Attendance.Application.Common.Interfaces;

namespace Skvia.Attendance.Api.Common.Middleware;

public class UserContextLoggingMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUserProvider currentUserProvider)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        var userId = currentUser?.Id != Guid.Empty ? currentUser?.Id.ToString() : "Anonymous";

        using (LogContext.PushProperty("UserId", userId))
        using (LogContext.PushProperty("TraceId", context.TraceIdentifier))
        {
            await next(context);
        }
    }
}
