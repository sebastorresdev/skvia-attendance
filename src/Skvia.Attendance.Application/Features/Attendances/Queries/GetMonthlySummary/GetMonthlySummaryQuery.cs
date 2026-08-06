using Skvia.Attendance.Application.Common.Messaging;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Attendances.Queries.GetMonthlySummary;

public record GetMonthlySummaryQuery(
    int Year,
    int Month,
    Guid? BranchId = null) : IQuery<ErrorOr<MonthlySummaryResponse>>;
