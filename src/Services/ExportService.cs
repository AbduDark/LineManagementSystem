using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using LineManagementSystem.Models;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace LineManagementSystem.Services;

public class ExportService
{
    private readonly DatabaseContext _db;

    public ExportService(DatabaseContext db)
    {
        _db = db;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    private XLColor GetProviderColor(TelecomProvider provider)
    {
        return provider switch
        {
            TelecomProvider.Vodafone => XLColor.FromHtml("#E60000"),
            TelecomProvider.Etisalat => XLColor.FromHtml("#7FBA00"),
            TelecomProvider.WE => XLColor.FromHtml("#6A1B9A"),
            TelecomProvider.Orange => XLColor.FromHtml("#FF7900"),
            _ => XLColor.White
        };
    }

    private string GetProviderColorPdf(TelecomProvider provider)
    {
        return provider switch
        {
            TelecomProvider.Vodafone => "#E60000",
            TelecomProvider.Etisalat => "#7FBA00",
            TelecomProvider.WE => "#6A1B9A",
            TelecomProvider.Orange => "#FF7900",
            _ => "#000000"
        };
    }

    public void ExportLinesToExcel(string filePath, List<PhoneLine> lines = null)
    {
        lines ??= _db.PhoneLines.ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("الخطوط");

        worksheet.RightToLeft = true;

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#37474F");
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Style.Font.FontSize = 12;
        headerRow.Height = 25;

        worksheet.Cell(1, 1).Value = "رقم الخط";
        worksheet.Cell(1, 2).Value = "اسم الشخص";
        worksheet.Cell(1, 3).Value = "الرقم القومي";
        worksheet.Cell(1, 4).Value = "ID داخلي";
        worksheet.Cell(1, 5).Value = "محفظة كاش";
        worksheet.Cell(1, 6).Value = "رقم المحفظة";
        worksheet.Cell(1, 7).Value = "المزود";
        worksheet.Cell(1, 8).Value = "المجموعة";
        worksheet.Cell(1, 9).Value = "التفاصيل";

        int row = 2;
        foreach (var line in lines)
        {
            var group = _db.LineGroups.Find(line.GroupId);
            var providerColor = GetProviderColor(group?.Provider ?? TelecomProvider.Vodafone);

            var currentRow = worksheet.Row(row);
            currentRow.Height = 22;

            worksheet.Cell(row, 1).Value = line.PhoneNumber;
            worksheet.Cell(row, 1).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 1).Style.Font.Bold = true;

            worksheet.Cell(row, 2).Value = line.PersonName;
            worksheet.Cell(row, 3).Value = line.NationalId;
            worksheet.Cell(row, 4).Value = line.InternalId;
            worksheet.Cell(row, 5).Value = line.HasCashWallet ? "نعم" : "لا";
            worksheet.Cell(row, 6).Value = line.CashWalletNumber ?? "";
            
            worksheet.Cell(row, 7).Value = group?.Provider.ToString() ?? "";
            worksheet.Cell(row, 7).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 7).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 7).Style.Font.Bold = true;

            worksheet.Cell(row, 8).Value = group?.Name ?? "";
            worksheet.Cell(row, 9).Value = line.Details ?? "";

            for (int col = 1; col <= 9; col++)
            {
                worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.Gray;
                worksheet.Cell(row, col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            row++;
        }

        worksheet.Columns().AdjustToContents();
        
        for (int col = 1; col <= 9; col++)
        {
            worksheet.Column(col).Width = worksheet.Column(col).Width > 30 ? 30 : worksheet.Column(col).Width;
        }

        workbook.SaveAs(filePath);
    }

    public void ExportGroupsToExcel(string filePath, List<LineGroup> groups = null)
    {
        groups ??= _db.LineGroups.ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("المجموعات");

        worksheet.RightToLeft = true;

        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#37474F");
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Style.Font.FontSize = 12;
        headerRow.Height = 25;

        worksheet.Cell(1, 1).Value = "اسم المجموعة";
        worksheet.Cell(1, 2).Value = "المزود";
        worksheet.Cell(1, 3).Value = "الحالة";
        worksheet.Cell(1, 4).Value = "عدد الخطوط";
        worksheet.Cell(1, 5).Value = "الموظف المسؤول";
        worksheet.Cell(1, 6).Value = "العميل";
        worksheet.Cell(1, 7).Value = "موعد التسليم";
        worksheet.Cell(1, 8).Value = "التفاصيل الإضافية";

        int row = 2;
        foreach (var group in groups)
        {
            var providerColor = GetProviderColor(group.Provider);
            var lineCount = _db.PhoneLines.Count(l => l.GroupId == group.Id);

            var currentRow = worksheet.Row(row);
            currentRow.Height = 22;

            worksheet.Cell(row, 1).Value = group.Name;
            worksheet.Cell(row, 1).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 1).Style.Font.Bold = true;

            worksheet.Cell(row, 2).Value = group.Provider.ToString();
            worksheet.Cell(row, 2).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 2).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 2).Style.Font.Bold = true;

            worksheet.Cell(row, 3).Value = group.Status.ToString();
            worksheet.Cell(row, 4).Value = lineCount;
            worksheet.Cell(row, 5).Value = group.ResponsibleEmployee ?? "";
            worksheet.Cell(row, 6).Value = group.Customer ?? "";
            worksheet.Cell(row, 7).Value = group.ExpectedDeliveryDate?.ToString("yyyy-MM-dd") ?? "";
            worksheet.Cell(row, 8).Value = group.AdditionalDetails ?? "";

            for (int col = 1; col <= 8; col++)
            {
                worksheet.Cell(row, col).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                worksheet.Cell(row, col).Style.Border.OutsideBorderColor = XLColor.Gray;
                worksheet.Cell(row, col).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            row++;
        }

        worksheet.Columns().AdjustToContents();
        
        for (int col = 1; col <= 8; col++)
        {
            worksheet.Column(col).Width = worksheet.Column(col).Width > 35 ? 35 : worksheet.Column(col).Width;
        }

        workbook.SaveAs(filePath);
    }

    public void ExportFullReportToExcel(string filePath)
    {
        using var workbook = new XLWorkbook();

        var groupsSheet = workbook.Worksheets.Add("المجموعات");
        groupsSheet.RightToLeft = true;

        var linesSheet = workbook.Worksheets.Add("الخطوط");
        linesSheet.RightToLeft = true;

        var statsSheet = workbook.Worksheets.Add("الإحصائيات");
        statsSheet.RightToLeft = true;

        CreateGroupsSheet(groupsSheet);
        CreateLinesSheet(linesSheet);
        CreateStatsSheet(statsSheet);

        workbook.SaveAs(filePath);
    }

    private void CreateGroupsSheet(IXLWorksheet worksheet)
    {
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#37474F");
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Style.Font.FontSize = 12;
        headerRow.Height = 25;

        worksheet.Cell(1, 1).Value = "اسم المجموعة";
        worksheet.Cell(1, 2).Value = "المزود";
        worksheet.Cell(1, 3).Value = "الحالة";
        worksheet.Cell(1, 4).Value = "عدد الخطوط";
        worksheet.Cell(1, 5).Value = "الموظف المسؤول";
        worksheet.Cell(1, 6).Value = "العميل";

        var groups = _db.LineGroups.ToList();
        int row = 2;
        foreach (var group in groups)
        {
            var providerColor = GetProviderColor(group.Provider);
            var lineCount = _db.PhoneLines.Count(l => l.GroupId == group.Id);

            worksheet.Cell(row, 1).Value = group.Name;
            worksheet.Cell(row, 1).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 1).Style.Font.Bold = true;

            worksheet.Cell(row, 2).Value = group.Provider.ToString();
            worksheet.Cell(row, 2).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 2).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 2).Style.Font.Bold = true;

            worksheet.Cell(row, 3).Value = group.Status.ToString();
            worksheet.Cell(row, 4).Value = lineCount;
            worksheet.Cell(row, 5).Value = group.ResponsibleEmployee ?? "";
            worksheet.Cell(row, 6).Value = group.Customer ?? "";

            row++;
        }

        worksheet.Columns().AdjustToContents();
    }

    private void CreateLinesSheet(IXLWorksheet worksheet)
    {
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#37474F");
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Style.Font.FontSize = 12;
        headerRow.Height = 25;

        worksheet.Cell(1, 1).Value = "رقم الخط";
        worksheet.Cell(1, 2).Value = "اسم الشخص";
        worksheet.Cell(1, 3).Value = "الرقم القومي";
        worksheet.Cell(1, 4).Value = "المزود";
        worksheet.Cell(1, 5).Value = "المجموعة";

        var lines = _db.PhoneLines.ToList();
        int row = 2;
        foreach (var line in lines)
        {
            var group = _db.LineGroups.Find(line.GroupId);
            var providerColor = GetProviderColor(group?.Provider ?? TelecomProvider.Vodafone);

            worksheet.Cell(row, 1).Value = line.PhoneNumber;
            worksheet.Cell(row, 1).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 1).Style.Font.Bold = true;

            worksheet.Cell(row, 2).Value = line.PersonName;
            worksheet.Cell(row, 3).Value = line.NationalId;
            
            worksheet.Cell(row, 4).Value = group?.Provider.ToString() ?? "";
            worksheet.Cell(row, 4).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 4).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 4).Style.Font.Bold = true;

            worksheet.Cell(row, 5).Value = group?.Name ?? "";

            row++;
        }

        worksheet.Columns().AdjustToContents();
    }

    private void CreateStatsSheet(IXLWorksheet worksheet)
    {
        var headerRow = worksheet.Row(1);
        headerRow.Style.Font.Bold = true;
        headerRow.Style.Fill.BackgroundColor = XLColor.FromHtml("#37474F");
        headerRow.Style.Font.FontColor = XLColor.White;
        headerRow.Style.Font.FontSize = 14;
        headerRow.Height = 30;

        worksheet.Cell(1, 1).Value = "الإحصائيات العامة";
        worksheet.Range(1, 1, 1, 2).Merge();

        worksheet.Cell(3, 1).Value = "البند";
        worksheet.Cell(3, 2).Value = "القيمة";
        worksheet.Row(3).Style.Font.Bold = true;

        worksheet.Cell(4, 1).Value = "إجمالي المجموعات";
        worksheet.Cell(4, 2).Value = _db.LineGroups.Count();

        worksheet.Cell(5, 1).Value = "إجمالي الخطوط";
        worksheet.Cell(5, 2).Value = _db.PhoneLines.Count();

        worksheet.Cell(7, 1).Value = "المزود";
        worksheet.Cell(7, 2).Value = "عدد المجموعات";
        worksheet.Cell(7, 3).Value = "عدد الخطوط";
        worksheet.Row(7).Style.Font.Bold = true;

        int row = 8;
        foreach (var provider in new[] { TelecomProvider.Vodafone, TelecomProvider.Etisalat, TelecomProvider.WE, TelecomProvider.Orange })
        {
            var providerColor = GetProviderColor(provider);
            var groupCount = _db.LineGroups.Count(g => g.Provider == provider);
            var lineCount = _db.PhoneLines.Count(l => _db.LineGroups.Any(g => g.Id == l.GroupId && g.Provider == provider));

            worksheet.Cell(row, 1).Value = provider.ToString();
            worksheet.Cell(row, 1).Style.Fill.BackgroundColor = providerColor;
            worksheet.Cell(row, 1).Style.Font.FontColor = XLColor.White;
            worksheet.Cell(row, 1).Style.Font.Bold = true;

            worksheet.Cell(row, 2).Value = groupCount;
            worksheet.Cell(row, 3).Value = lineCount;

            row++;
        }

        worksheet.Columns().AdjustToContents();
    }

    public void ExportToPdf(string filePath)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                page.Header()
                    .Text("تقرير نظام إدارة الخطوط")
                    .SemiBold().FontSize(20).FontColor(QuestPDF.Helpers.Colors.Blue.Medium);

                page.Content()
                    .Column(column =>
                    {
                        column.Item().Text($"التاريخ: {DateTime.Now:yyyy-MM-dd HH:mm}").FontSize(10);
                        column.Item().PaddingVertical(10);

                        column.Item().Text("الإحصائيات العامة").Bold().FontSize(14);
                        column.Item().PaddingVertical(5);

                        column.Item().Text($"إجمالي المجموعات: {_db.LineGroups.Count()}");
                        column.Item().Text($"إجمالي الخطوط: {_db.PhoneLines.Count()}");

                        column.Item().PaddingVertical(10);
                        column.Item().Text("إحصائيات حسب المزود").Bold().FontSize(14);
                        column.Item().PaddingVertical(5);

                        foreach (var provider in new[] { TelecomProvider.Vodafone, TelecomProvider.Etisalat, TelecomProvider.WE, TelecomProvider.Orange })
                        {
                            var groupCount = _db.LineGroups.Count(g => g.Provider == provider);
                            var lineCount = _db.PhoneLines.Count(l => _db.LineGroups.Any(g => g.Id == l.GroupId && g.Provider == provider));

                            column.Item().Text($"{provider}: {groupCount} مجموعة، {lineCount} خط");
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("صفحة ");
                        x.CurrentPageNumber();
                    });
            });
        });

        document.GeneratePdf(filePath);
    }
}
