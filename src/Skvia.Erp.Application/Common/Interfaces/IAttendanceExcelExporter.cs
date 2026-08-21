using Skvia.Erp.Application.Features.Attendances.Queries.ExportAttendancesExcel;

namespace Skvia.Erp.Application.Common.Interfaces;

public interface IAttendanceExcelExporter
{
    byte[] ExportAttendances(
        IReadOnlyList<AttendanceExportDto> attendances,
        DateOnly startDate,
        DateOnly endDate);
}

