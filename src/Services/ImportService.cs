using ClosedXML.Excel;
using LineManagementSystem.Models;
using System.Text.RegularExpressions;

namespace LineManagementSystem.Services;

public class ImportService
{
    public class ImportResult
    {
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<PhoneLine> ImportedLines { get; set; } = new();
    }

    public class ColumnMapping
    {
        public int NameColumn { get; set; }
        public int NationalIdColumn { get; set; }
        public int PhoneNumberColumn { get; set; }
    }

    public class CustomImportSettings
    {
        public bool HasHeader { get; set; }
        public int NameColumn { get; set; }
        public int NationalIdColumn { get; set; }
        public int PhoneNumberColumn { get; set; }
        public int? InternalIdColumn { get; set; }
        public int? HasCashWalletColumn { get; set; }
        public int? CashWalletNumberColumn { get; set; }
    }

    public ImportResult ImportFromExcel(string filePath, int groupId)
    {
        var result = new ImportResult();

        try
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                result.Errors.Add("الملف لا يحتوي على أي أوراق عمل");
                return result;
            }

            var mapping = DetectColumns(worksheet);
            
            if (mapping == null)
            {
                result.Errors.Add("لم يتم العثور على الأعمدة المطلوبة (الاسم، الرقم القومي، رقم الخط)");
                return result;
            }

            var firstDataRow = 2;
            var hasHeader = HasHeaderRow(worksheet);
            if (!hasHeader)
            {
                firstDataRow = 1;
            }

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            for (int row = firstDataRow; row <= lastRow; row++)
            {
                try
                {
                    var name = worksheet.Cell(row, mapping.NameColumn).GetString().Trim();
                    var nationalId = worksheet.Cell(row, mapping.NationalIdColumn).GetString().Trim();
                    var phoneNumber = worksheet.Cell(row, mapping.PhoneNumberColumn).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(name) || 
                        string.IsNullOrWhiteSpace(nationalId) || 
                        string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        continue;
                    }

                    phoneNumber = NormalizePhoneNumber(phoneNumber);
                    nationalId = NormalizeNationalId(nationalId);

                    if (!IsValidNationalId(nationalId))
                    {
                        result.Errors.Add($"صف {row}: رقم قومي غير صحيح - {nationalId}");
                        result.FailedCount++;
                        continue;
                    }

                    if (!IsValidPhoneNumber(phoneNumber))
                    {
                        result.Errors.Add($"صف {row}: رقم خط غير صحيح - {phoneNumber}");
                        result.FailedCount++;
                        continue;
                    }

                    var internalId = (row - (hasHeader ? 1 : 0)).ToString();

                    var phoneLine = new PhoneLine
                    {
                        Name = name,
                        NationalId = nationalId,
                        PhoneNumber = phoneNumber,
                        InternalId = internalId,
                        GroupId = groupId
                    };

                    result.ImportedLines.Add(phoneLine);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"صف {row}: {ex.Message}");
                    result.FailedCount++;
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"خطأ في قراءة الملف: {ex.Message}");
        }

        return result;
    }

    public ImportResult ImportFromExcelWithCustomSettings(string filePath, int groupId, CustomImportSettings settings)
    {
        var result = new ImportResult();

        try
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                result.Errors.Add("الملف لا يحتوي على أي أوراق عمل");
                return result;
            }

            var firstDataRow = settings.HasHeader ? 2 : 1;
            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            for (int row = firstDataRow; row <= lastRow; row++)
            {
                try
                {
                    var name = worksheet.Cell(row, settings.NameColumn).GetString().Trim();
                    var nationalId = worksheet.Cell(row, settings.NationalIdColumn).GetString().Trim();
                    var phoneNumber = worksheet.Cell(row, settings.PhoneNumberColumn).GetString().Trim();

                    if (string.IsNullOrWhiteSpace(name) || 
                        string.IsNullOrWhiteSpace(nationalId) || 
                        string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        continue;
                    }

                    phoneNumber = NormalizePhoneNumber(phoneNumber);
                    nationalId = NormalizeNationalId(nationalId);

                    if (!IsValidNationalId(nationalId))
                    {
                        result.Errors.Add($"صف {row}: رقم قومي غير صحيح - {nationalId}");
                        result.FailedCount++;
                        continue;
                    }

                    if (!IsValidPhoneNumber(phoneNumber))
                    {
                        result.Errors.Add($"صف {row}: رقم خط غير صحيح - {phoneNumber}");
                        result.FailedCount++;
                        continue;
                    }

                    var internalId = "";
                    if (settings.InternalIdColumn.HasValue)
                    {
                        internalId = worksheet.Cell(row, settings.InternalIdColumn.Value).GetString().Trim();
                    }
                    else
                    {
                        internalId = (row - (settings.HasHeader ? 1 : 0)).ToString();
                    }

                    var hasCashWallet = false;
                    if (settings.HasCashWalletColumn.HasValue)
                    {
                        var walletValue = worksheet.Cell(row, settings.HasCashWalletColumn.Value).GetString().Trim().ToLower();
                        hasCashWallet = walletValue == "نعم" || walletValue == "yes" || walletValue == "1" || walletValue == "true";
                    }

                    var cashWalletNumber = "";
                    if (settings.CashWalletNumberColumn.HasValue)
                    {
                        cashWalletNumber = worksheet.Cell(row, settings.CashWalletNumberColumn.Value).GetString().Trim();
                    }

                    var phoneLine = new PhoneLine
                    {
                        Name = name,
                        NationalId = nationalId,
                        PhoneNumber = phoneNumber,
                        InternalId = internalId,
                        HasCashWallet = hasCashWallet,
                        CashWalletNumber = !string.IsNullOrWhiteSpace(cashWalletNumber) ? cashWalletNumber : null,
                        GroupId = groupId
                    };

                    result.ImportedLines.Add(phoneLine);
                    result.SuccessCount++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"صف {row}: {ex.Message}");
                    result.FailedCount++;
                }
            }
        }
        catch (Exception ex)
        {
            result.Errors.Add($"خطأ في قراءة الملف: {ex.Message}");
        }

        return result;
    }

    private ColumnMapping? DetectColumns(IXLWorksheet worksheet)
    {
        var lastColumn = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

        if (lastColumn < 3)
            return null;

        var hasHeader = HasHeaderRow(worksheet);
        
        if (hasHeader)
        {
            return DetectColumnsFromHeader(worksheet, lastColumn);
        }
        else
        {
            return DetectColumnsFromData(worksheet, lastColumn);
        }
    }

    private bool HasHeaderRow(IXLWorksheet worksheet)
    {
        var firstRow = worksheet.Row(1);
        var lastColumn = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;
        
        int textCellCount = 0;
        int totalCells = 0;

        for (int col = 1; col <= Math.Min(lastColumn, 10); col++)
        {
            var cellValue = firstRow.Cell(col).GetString();
            if (!string.IsNullOrWhiteSpace(cellValue))
            {
                totalCells++;
                if (!IsNumericOnly(cellValue))
                {
                    textCellCount++;
                }
            }
        }

        return totalCells > 0 && textCellCount >= totalCells / 2;
    }

    private ColumnMapping? DetectColumnsFromHeader(IXLWorksheet worksheet, int lastColumn)
    {
        var mapping = new ColumnMapping();
        var headerRow = worksheet.Row(1);

        for (int col = 1; col <= lastColumn; col++)
        {
            var header = headerRow.Cell(col).GetString().ToLower().Trim()
                .Replace(" ", "").Replace("-", "").Replace("_", "");
            
            if (IsNameHeader(header))
            {
                mapping.NameColumn = col;
            }
            else if (IsNationalIdHeader(header))
            {
                mapping.NationalIdColumn = col;
            }
            else if (IsPhoneNumberHeader(header))
            {
                mapping.PhoneNumberColumn = col;
            }
        }

        if (mapping.NameColumn > 0 && mapping.NationalIdColumn > 0 && mapping.PhoneNumberColumn > 0)
        {
            return mapping;
        }

        return null;
    }

    private bool IsNameHeader(string header)
    {
        var nameKeywords = new[] { 
            "اسم", "name", "الاسم", "fullname", "person", "شخص", "صاحب", "owner" 
        };
        return nameKeywords.Any(keyword => header.Contains(keyword));
    }

    private bool IsNationalIdHeader(string header)
    {
        var nationalIdKeywords = new[] { 
            "قومي", "national", "رقمقومي", "الرقمالقومي", "nationalid", "id", "ssn", "socialsecurity",
            "بطاقة", "هوية", "identity", "cardnumber", "idcard"
        };
        return nationalIdKeywords.Any(keyword => header.Contains(keyword)) && 
               !header.Contains("phone") && !header.Contains("mobile") && !header.Contains("tel");
    }

    private bool IsPhoneNumberHeader(string header)
    {
        var phoneKeywords = new[] { 
            "رقم", "phone", "خط", "الرقم", "mobile", "cell", "telephone", "tel", "contact",
            "موبايل", "جوال", "هاتف", "number"
        };
        
        var hasPhoneKeyword = phoneKeywords.Any(keyword => header.Contains(keyword));
        var isNotNationalId = !header.Contains("قومي") && !header.Contains("national") && 
                              !header.Contains("ssn") && !header.Contains("identity") && 
                              !header.Contains("هوية") && !header.Contains("بطاقة");
        
        return hasPhoneKeyword && isNotNationalId;
    }

    private ColumnMapping? DetectColumnsFromData(IXLWorksheet worksheet, int lastColumn)
    {
        var mapping = new ColumnMapping();
        var sampleSize = Math.Min(5, worksheet.LastRowUsed()?.RowNumber() ?? 0);

        var columnScores = new Dictionary<int, Dictionary<string, int>>();
        
        for (int col = 1; col <= lastColumn; col++)
        {
            columnScores[col] = new Dictionary<string, int>
            {
                ["name"] = 0,
                ["nationalId"] = 0,
                ["phone"] = 0
            };
        }

        for (int row = 1; row <= sampleSize; row++)
        {
            for (int col = 1; col <= lastColumn; col++)
            {
                var value = worksheet.Cell(row, col).GetString().Trim();
                
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                var cleanValue = value.Replace(" ", "").Replace("-", "");

                if (IsArabicOrEnglishText(value) && !IsNumericOnly(cleanValue))
                {
                    columnScores[col]["name"] += 10;
                }

                if (cleanValue.Length == 14 && IsNumericOnly(cleanValue))
                {
                    columnScores[col]["nationalId"] += 20;
                }

                if (cleanValue.Length == 11 && IsNumericOnly(cleanValue) && cleanValue.StartsWith("01"))
                {
                    columnScores[col]["phone"] += 20;
                }
            }
        }

        var nameCol = columnScores.OrderByDescending(x => x.Value["name"]).FirstOrDefault();
        var nationalIdCol = columnScores.OrderByDescending(x => x.Value["nationalId"]).FirstOrDefault();
        var phoneCol = columnScores.OrderByDescending(x => x.Value["phone"]).FirstOrDefault();

        if (nameCol.Value["name"] > 0 && nationalIdCol.Value["nationalId"] > 0 && phoneCol.Value["phone"] > 0)
        {
            mapping.NameColumn = nameCol.Key;
            mapping.NationalIdColumn = nationalIdCol.Key;
            mapping.PhoneNumberColumn = phoneCol.Key;
            return mapping;
        }

        return null;
    }

    private bool IsArabicOrEnglishText(string text)
    {
        return Regex.IsMatch(text, @"[\u0600-\u06FFa-zA-Z]");
    }

    private bool IsNumericOnly(string text)
    {
        return Regex.IsMatch(text.Replace(" ", "").Replace("-", ""), @"^\d+$");
    }

    private string NormalizePhoneNumber(string phoneNumber)
    {
        var cleaned = phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
        
        if (cleaned.StartsWith("+2"))
        {
            cleaned = cleaned.Substring(2);
        }
        else if (cleaned.StartsWith("2") && cleaned.Length == 12)
        {
            cleaned = cleaned.Substring(1);
        }

        return cleaned;
    }

    private string NormalizeNationalId(string nationalId)
    {
        return nationalId.Replace(" ", "").Replace("-", "");
    }

    private bool IsValidNationalId(string nationalId)
    {
        return nationalId.Length == 14 && IsNumericOnly(nationalId);
    }

    private bool IsValidPhoneNumber(string phoneNumber)
    {
        return phoneNumber.Length == 11 && phoneNumber.StartsWith("01") && IsNumericOnly(phoneNumber);
    }
}
