using Skvia.Attendance.Application.Features.Branches.DTOs;

namespace Skvia.Attendance.Application.Features.Branches.Queries.GetBranchById;

public record GetBranchByIdQuery(Guid BranchId) : IQuery<ErrorOr<BranchDetailResponse>>;
