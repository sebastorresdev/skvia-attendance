using ClosedXML.Excel;
using Skvia.Erp.Application.Common.Interfaces;
using Skvia.Erp.Application.Features.Attendances.Queries.ExportAttendancesExcel;

namespace Skvia.Erp.Infrastructure.Services;

public class AttendanceExcelExporter : IAttendanceExcelExporter
{
    public byte[] ExportAttendances(
        IReadOnlyList<AttendanceExportDto> attendances,
        DateOnly startDate,
        DateOnly endDate)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Reporte de Asistencias");

        // 1. Title Banner
        worksheet.Cell("A1").Value = "REPORTE DE ASISTENCIAS DE PERSONAL";
        worksheet.Range("A1:I1").Merge();
        worksheet.Row(1).Height = 35;
        var titleCell = worksheet.Cell("A1");
        titleCell.Style.Font.Bold = true;
        titleCell.Style.Font.FontSize = 16;
        titleCell.Style.Font.FontColor = XLColor.White;
        titleCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#1E293B"); // Slate 800
        titleCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        titleCell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        // 2. Metadata Section
        worksheet.Cell("A3").Value = "Rango de Fechas:";
        worksheet.Cell("B3").Value = $"{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";
        worksheet.Cell("A3").Style.Font.Bold = true;

        worksheet.Cell("D3").Value = "Fecha de Emisión:";
        worksheet.Cell("E3").Value = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        worksheet.Cell("D3").Style.Font.Bold = true;

        worksheet.Cell("G3").Value = "Total Registros:";
        worksheet.Cell("H3").Value = attendances.Count;
        worksheet.Cell("G3").Style.Font.Bold = true;

        // 3. Table Headers
        var headers = new string[]
        {
            "N°", "Fecha", "Código", "Empleado", "Sede", "Entrada", "Salida", "Min. Tardanza", "Estado"
        };

        int headerRow = 5;
        for (int col = 0; col < headers.Length; col++)
        {
            var cell = worksheet.Cell(headerRow, col + 1);
            cell.Value = headers[col];
            cell.Style.Font.Bold = true;
            cell.Style.Font.FontSize = 11;
            cell.Style.Font.FontColor = XLColor.White;
            cell.Style.Fill.BackgroundColor = XLColor.FromHtml("#334155"); // Slate 700
            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        }
        worksheet.Row(headerRow).Height = 24;

        // 4. Data Rows
        int currentRow = 6;
        int index = 1;
        int totalLateCount = 0;
        int totalMinutesLate = 0;

        foreach (var att in attendances)
        {
            worksheet.Cell(currentRow, 1).Value = index++;
            worksheet.Cell(currentRow, 2).Value = att.Date.ToString("dd/MM/yyyy");
            worksheet.Cell(currentRow, 3).Value = att.EmployeeCode;
            worksheet.Cell(currentRow, 4).Value = att.EmployeeName;
            worksheet.Cell(currentRow, 5).Value = att.BranchName;
            worksheet.Cell(currentRow, 6).Value = att.CheckIn.ToString("HH:mm:ss");
            worksheet.Cell(currentRow, 7).Value = att.CheckOut.HasValue ? att.CheckOut.Value.ToString("HH:mm:ss") : "-";
            worksheet.Cell(currentRow, 8).Value = att.MinutesLate;

            var statusCell = worksheet.Cell(currentRow, 9);
            if (att.IsLate)
            {
                totalLateCount++;
                totalMinutesLate += att.MinutesLate;
                statusCell.Value = "Tarde";
                statusCell.Style.Font.FontColor = XLColor.FromHtml("#991B1B"); // Red 800
                statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#FEE2E2"); // Red 100
                statusCell.Style.Font.Bold = true;
            }
            else
            {
                statusCell.Value = "Puntual";
                statusCell.Style.Font.FontColor = XLColor.FromHtml("#166534"); // Green 800
                statusCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#DCFCE7"); // Green 100
                statusCell.Style.Font.Bold = true;
            }

            // Alignments
            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(currentRow, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(currentRow, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(currentRow, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(currentRow, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            statusCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            // Borders
            var rowRange = worksheet.Range(currentRow, 1, currentRow, 9);
            rowRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.OutsideBorderColor = XLColor.FromHtml("#E2E8F0");
            rowRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            rowRange.Style.Border.InsideBorderColor = XLColor.FromHtml("#E2E8F0");

            currentRow++;
        }

        // 5. Summary Row
        if (attendances.Count > 0)
        {
            worksheet.Cell(currentRow, 1).Value = "RESUMEN:";
            worksheet.Range(currentRow, 1, currentRow, 7).Merge();
            worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(currentRow, 8).Value = totalMinutesLate;
            worksheet.Cell(currentRow, 8).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            worksheet.Cell(currentRow, 9).Value = $"{totalLateCount} Tardanza(s)";
            worksheet.Cell(currentRow, 9).Style.Font.Bold = true;
            worksheet.Cell(currentRow, 9).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var summaryRange = worksheet.Range(currentRow, 1, currentRow, 9);
            summaryRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8FAFC");
            summaryRange.Style.Border.TopBorder = XLBorderStyleValues.Medium;
        }

        worksheet.Columns().AdjustToContents();
        
        // Extra padding for aesthetics
        foreach (var col in worksheet.ColumnsUsed())
        {
            col.Width = Math.Max(col.Width + 3, 12);
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

