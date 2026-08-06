using Skvia.Attendance.Application.Features.Attendances.Queries.ExportAttendancesExcel;

namespace Skvia.Attendance.Application.Common.Interfaces;

public interface IAttendanceExcelExporter
{
    byte[] ExportAttendances(
        IReadOnlyList<AttendanceExportDto> attendances,
        DateOnly startDate,
        DateOnly endDate);
}
