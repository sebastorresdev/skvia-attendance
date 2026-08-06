using Skvia.Attendance.Application.Common.Messaging;
using Skvia.Attendance.Application.Features.Justifications.DTOs;
using Skvia.Attendance.Domain.Justifications;
using ErrorOr;

namespace Skvia.Attendance.Application.Features.Justifications.Queries.GetJustifications;

public record GetJustificationsQuery(
    DateOnly? StartDate = null,
    DateOnly? EndDate = null,
    Guid? EmployeeId = null,
    JustificationStatus? Status = null,
    Guid? BranchId = null) : IQuery<ErrorOr<List<JustificationResponse>>>;
