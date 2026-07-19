using Skvia.Attendance.Application.Common.Models;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUser GetCurrentUser();
}
