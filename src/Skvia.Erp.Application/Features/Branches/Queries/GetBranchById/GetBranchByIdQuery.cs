using Skvia.Erp.Application.Common.Messaging;
using ErrorOr;
using Skvia.Erp.Application.Features.Branches.DTOs;

namespace Skvia.Erp.Application.Features.Branches.Queries.GetBranchById;

public record GetBranchByIdQuery(Guid BranchId) : IQuery<ErrorOr<BranchDetailResponse>>;


