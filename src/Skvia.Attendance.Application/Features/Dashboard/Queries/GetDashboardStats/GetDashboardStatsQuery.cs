using Skvia.Attendance.Application.Common.Messaging;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Dashboard.Queries.GetDashboardStats;

public record GetDashboardStatsQuery(Guid? BranchId = null) : IQuery<ErrorOr<DashboardStatsResponse>>;
