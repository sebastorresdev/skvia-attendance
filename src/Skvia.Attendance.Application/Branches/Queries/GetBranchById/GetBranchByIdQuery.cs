using Skvia.Attendance.Application.Branches.DTOs;

namespace Skvia.Attendance.Application.Branches.Queries.GetBranchById;

public record GetBranchByIdQuery(Guid BranchId) : IQuery<ErrorOr<BranchDetailResponse>>;
