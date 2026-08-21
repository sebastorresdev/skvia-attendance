using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Attendances.Queries.GetMonthlySummary;

public record GetMonthlySummaryQuery(
    int Year,
    int Month,
    Guid? BranchId = null) : IQuery<ErrorOr<MonthlySummaryResponse>>;

