using Skvia.Erp.Application.Common.Messaging;
using Skvia.Erp.Application.Common.Interfaces;
using ErrorOr;

namespace Skvia.Erp.Application.Features.Attendances.Queries.ExportAttendancesExcel;

public record ExportAttendancesExcelQuery(
    DateOnly StartDate,
    DateOnly EndDate,
    Guid? BranchId = null,
    string? EmployeeSearch = null,
    Guid? EmployeeId = null,
    string? StatusFilter = null) : IQuery<ErrorOr<ExportExcelResult>>;

public record ExportExcelResult(byte[] FileContents, string ContentType, string FileName);


