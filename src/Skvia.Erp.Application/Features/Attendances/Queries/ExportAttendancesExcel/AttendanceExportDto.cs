namespace Skvia.Erp.Application.Features.Attendances.Queries.ExportAttendancesExcel;

public record AttendanceExportDto(
    DateOnly Date,
    string EmployeeCode,
    string EmployeeName,
    string BranchName,
    DateTimeOffset CheckIn,
    DateTimeOffset? CheckOut,
    int MinutesLate,
    bool IsLate);

