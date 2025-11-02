
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using LineManagementSystem.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LineManagementSystem.Services;

public class ExportService
{
    public void ExportGroupsToExcel(IEnumerable<LineGroup> groups, string filePath)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("المجموعات");

        // إضافة العناوين
        worksheet.Cell(1, 1).Value = "اسم المجموعة";
        worksheet.Cell(1, 2).Value = "الشركة";
        worksheet.Cell(1, 3).Value = "عدد الخطوط";
        worksheet.Cell(1, 4).Value = "تاريخ التجديد";
        worksheet.Cell(1, 5).Value = "الموظف المسؤول";
        worksheet.Cell(1, 6).Value = "العميل";
        worksheet.Cell(1, 7).Value = "حالة التسليم";
        worksheet.Cell(1, 8).Value = "ملاحظات";

        // تنسيق العناوين
        var headerRange = worksheet.Range(1, 1, 1, 8);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // إضافة البيانات
        int row = 2;
        foreach (var group in groups)
        {
            worksheet.Cell(row, 1).Value = group.Name;
            worksheet.Cell(row, 2).Value = group.Provider.GetArabicName();
            worksheet.Cell(row, 3).Value = group.Lines?.Count ?? 0;
            worksheet.Cell(row, 4).Value = group.RenewalDue?.ToString("yyyy-MM-dd") ?? "";
            worksheet.Cell(row, 5).Value = group.AssignedToEmployee ?? "";
            worksheet.Cell(row, 6).Value = group.AssignedCustomer ?? "";
            worksheet.Cell(row, 7).Value = group.IsHandedOver ? "تم التسليم" : "لم يتم";
            worksheet.Cell(row, 8).Value = group.AdditionalDetails ?? "";
            row++;
        }

        // ضبط عرض الأعمدة
        worksheet.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
    }

    public void ExportLinesToExcel(IEnumerable<PhoneLine> lines, string groupName, string filePath)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("الخطوط");

        // إضافة العناوين
        worksheet.Cell(1, 1).Value = "اسم الشخص";
        worksheet.Cell(1, 2).Value = "الرقم القومي";
        worksheet.Cell(1, 3).Value = "رقم الخط";
        worksheet.Cell(1, 4).Value = "الرقم الداخلي";
        worksheet.Cell(1, 5).Value = "رقم المحفظة";
        worksheet.Cell(1, 6).Value = "مستوى التأكيد";
        worksheet.Cell(1, 7).Value = "ملاحظات";

        // تنسيق العناوين
        var headerRange = worksheet.Range(1, 1, 1, 7);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightGreen;
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        // إضافة البيانات
        int row = 2;
        foreach (var line in lines)
        {
            worksheet.Cell(row, 1).Value = line.Name ?? "";
            worksheet.Cell(row, 2).Value = line.NationalId ?? "";
            worksheet.Cell(row, 3).Value = line.PhoneNumber ?? "";
            worksheet.Cell(row, 4).Value = line.InternalId ?? "";
            worksheet.Cell(row, 5).Value = line.CashWalletNumber ?? "";
            worksheet.Cell(row, 6).Value = GetConfirmationLevelText(line.ConfirmationLevel);
            worksheet.Cell(row, 7).Value = line.Details ?? "";
            row++;
        }

        // ضبط عرض الأعمدة
        worksheet.Columns().AdjustToContents();

        workbook.SaveAs(filePath);
    }

    public void ExportGroupsToPDF(IEnumerable<LineGroup> groups, string filePath)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Header()
                    .Text("تقرير المجموعات")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        foreach (var group in groups)
                        {
                            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10).Column(groupColumn =>
                            {
                                groupColumn.Item().Text($"المجموعة: {group.Name}").FontSize(14).Bold();
                                groupColumn.Item().Text($"الشركة: {group.Provider.GetArabicName()}");
                                groupColumn.Item().Text($"عدد الخطوط: {group.Lines?.Count ?? 0}");
                                if (group.RenewalDue.HasValue)
                                    groupColumn.Item().Text($"تاريخ التجديد: {group.RenewalDue.Value:yyyy-MM-dd}");
                                if (!string.IsNullOrEmpty(group.AssignedToEmployee))
                                    groupColumn.Item().Text($"الموظف: {group.AssignedToEmployee}");
                                if (!string.IsNullOrEmpty(group.AssignedCustomer))
                                    groupColumn.Item().Text($"العميل: {group.AssignedCustomer}");
                                groupColumn.Item().Text($"حالة التسليم: {(group.IsHandedOver ? "تم التسليم" : "لم يتم")}");
                            });
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("صفحة ");
                        x.CurrentPageNumber();
                        x.Span(" من ");
                        x.TotalPages();
                    });
            });
        })
        .GeneratePdf(filePath);
    }

    public void ExportLinesToPDF(IEnumerable<PhoneLine> lines, string groupName, string filePath)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Arial"));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text($"تقرير خطوط المجموعة: {groupName}")
                            .FontSize(18)
                            .Bold()
                            .AlignCenter();
                        column.Item().Text($"إجمالي الخطوط: {lines.Count()}")
                            .FontSize(12)
                            .AlignCenter();
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Spacing(10);

                        foreach (var line in lines)
                        {
                            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(10).Column(lineColumn =>
                            {
                                if (!string.IsNullOrEmpty(line.Name))
                                    lineColumn.Item().Text($"الاسم: {line.Name}").Bold();
                                if (!string.IsNullOrEmpty(line.NationalId))
                                    lineColumn.Item().Text($"الرقم القومي: {line.NationalId}");
                                if (!string.IsNullOrEmpty(line.PhoneNumber))
                                    lineColumn.Item().Text($"رقم الخط: {line.PhoneNumber}");
                                if (!string.IsNullOrEmpty(line.InternalId))
                                    lineColumn.Item().Text($"الرقم الداخلي: {line.InternalId}");
                                if (!string.IsNullOrEmpty(line.CashWalletNumber))
                                    lineColumn.Item().Text($"رقم المحفظة: {line.CashWalletNumber}");
                                lineColumn.Item().Text($"مستوى التأكيد: {GetConfirmationLevelText(line.ConfirmationLevel)}");
                            });
                        }
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("صفحة ");
                        x.CurrentPageNumber();
                        x.Span(" من ");
                        x.TotalPages();
                    });
            });
        })
        .GeneratePdf(filePath);
    }

    private string GetConfirmationLevelText(int level)
    {
        return level switch
        {
            1 => "⭐",
            2 => "⭐⭐",
            3 => "⭐⭐⭐",
            _ => "غير محدد"
        };
    }
}
