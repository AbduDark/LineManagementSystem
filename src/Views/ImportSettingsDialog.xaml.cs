using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;
using LineManagementSystem.Services;

namespace LineManagementSystem.Views;

public partial class ImportSettingsDialog : Window
{
    public class ImportSettings
    {
        public bool HasHeader { get; set; }
        public int? NameColumn { get; set; }
        public int? NationalIdColumn { get; set; }
        public int? PhoneNumberColumn { get; set; }
        public int? InternalIdColumn { get; set; }
        public int? HasCashWalletColumn { get; set; }
        public int? CashWalletNumberColumn { get; set; }
        public int? LineSystemColumn { get; set; }
    }

    public ImportSettings? Settings { get; private set; }
    private readonly string _excelFilePath;
    private readonly ImportService _importService;

    public ImportSettingsDialog(string excelFilePath)
    {
        InitializeComponent();
        _excelFilePath = excelFilePath;
        _importService = new ImportService();
        LoadColumnsFromExcel();
    }

    private void LoadColumnsFromExcel()
    {
        try
        {
            using var workbook = new XLWorkbook(_excelFilePath);
            var worksheet = workbook.Worksheets.FirstOrDefault();

            if (worksheet == null)
            {
                MessageBox.Show("الملف لا يحتوي على أي أوراق عمل", "خطأ", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
                DialogResult = false;
                Close();
                return;
            }

            var lastColumn = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;

            // إنشاء قائمة الأعمدة
            var columns = new List<string>();
            for (int i = 1; i <= lastColumn; i++)
            {
                var headerValue = worksheet.Cell(1, i).GetString();
                if (!string.IsNullOrWhiteSpace(headerValue))
                {
                    columns.Add($"عمود {i}: {headerValue}");
                }
                else
                {
                    columns.Add($"عمود {i}");
                }
            }

            // تعبئة ComboBoxes
            NameColumnComboBox.ItemsSource = columns;
            NationalIdColumnComboBox.ItemsSource = columns;
            PhoneNumberColumnComboBox.ItemsSource = columns;
            InternalIdColumnComboBox.ItemsSource = columns;
            HasCashWalletColumnComboBox.ItemsSource = columns;
            CashWalletNumberColumnComboBox.ItemsSource = columns;
            LineSystemColumnComboBox.ItemsSource = columns;

            // اكتشاف الأعمدة تلقائياً
            var detectedMapping = _importService.DetectAllColumns(_excelFilePath);

            if (detectedMapping != null)
            {
                // تحديد الأعمدة الأساسية تلقائياً
                if (detectedMapping.NameColumn > 0)
                {
                    NameColumnComboBox.SelectedIndex = detectedMapping.NameColumn - 1;
                }
                
                if (detectedMapping.NationalIdColumn > 0)
                {
                    NationalIdColumnComboBox.SelectedIndex = detectedMapping.NationalIdColumn - 1;
                }
                
                if (detectedMapping.PhoneNumberColumn > 0)
                {
                    PhoneNumberColumnComboBox.SelectedIndex = detectedMapping.PhoneNumberColumn - 1;
                }

                // تحديد الأعمدة الاختيارية إن وُجدت
                if (detectedMapping.InternalIdColumn.HasValue && detectedMapping.InternalIdColumn.Value > 0)
                {
                    InternalIdColumnCheckBox.IsChecked = true;
                    InternalIdColumnComboBox.SelectedIndex = detectedMapping.InternalIdColumn.Value - 1;
                }

                if (detectedMapping.HasCashWalletColumn.HasValue && detectedMapping.HasCashWalletColumn.Value > 0)
                {
                    HasCashWalletColumnCheckBox.IsChecked = true;
                    HasCashWalletColumnComboBox.SelectedIndex = detectedMapping.HasCashWalletColumn.Value - 1;
                }

                if (detectedMapping.CashWalletNumberColumn.HasValue && detectedMapping.CashWalletNumberColumn.Value > 0)
                {
                    CashWalletNumberColumnCheckBox.IsChecked = true;
                    CashWalletNumberColumnComboBox.SelectedIndex = detectedMapping.CashWalletNumberColumn.Value - 1;
                }

                if (detectedMapping.LineSystemColumn.HasValue && detectedMapping.LineSystemColumn.Value > 0)
                {
                    LineSystemColumnCheckBox.IsChecked = true;
                    LineSystemColumnComboBox.SelectedIndex = detectedMapping.LineSystemColumn.Value - 1;
                }

                // عرض رسالة للمستخدم
                MessageBox.Show(
                    "✅ تم اكتشاف الأعمدة تلقائياً!\n\n" +
                    "يرجى مراجعة التحديدات والتأكد من صحتها قبل الاستيراد.",
                    "اكتشاف تلقائي",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                // إذا فشل الاكتشاف، استخدم الإعدادات الافتراضية
                NameColumnComboBox.SelectedIndex = 0;
                if (columns.Count > 1) NationalIdColumnComboBox.SelectedIndex = 1;
                if (columns.Count > 2) PhoneNumberColumnComboBox.SelectedIndex = 2;

                MessageBox.Show(
                    "⚠️ لم يتم اكتشاف الأعمدة تلقائياً\n\n" +
                    "يرجى تحديد الأعمدة يدوياً",
                    "تنبيه",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في قراءة الملف: {ex.Message}", "خطأ", 
                MessageBoxButton.OK, MessageBoxImage.Error);
            DialogResult = false;
            Close();
        }
    }

    private void OptionalColumn_Changed(object sender, RoutedEventArgs e)
    {
        if (NameColumnCheckBox != null && NameColumnComboBox != null)
        {
            NameColumnComboBox.IsEnabled = NameColumnCheckBox.IsChecked == true;
        }

        if (NationalIdColumnCheckBox != null && NationalIdColumnComboBox != null)
        {
            NationalIdColumnComboBox.IsEnabled = NationalIdColumnCheckBox.IsChecked == true;
        }

        if (PhoneNumberColumnCheckBox != null && PhoneNumberColumnComboBox != null)
        {
            PhoneNumberColumnComboBox.IsEnabled = PhoneNumberColumnCheckBox.IsChecked == true;
        }

        if (InternalIdColumnCheckBox != null && InternalIdColumnComboBox != null)
        {
            InternalIdColumnComboBox.IsEnabled = InternalIdColumnCheckBox.IsChecked == true;
        }

        if (HasCashWalletColumnCheckBox != null && HasCashWalletColumnComboBox != null)
        {
            HasCashWalletColumnComboBox.IsEnabled = HasCashWalletColumnCheckBox.IsChecked == true;
        }

        if (CashWalletNumberColumnCheckBox != null && CashWalletNumberColumnComboBox != null)
        {
            CashWalletNumberColumnComboBox.IsEnabled = CashWalletNumberColumnCheckBox.IsChecked == true;
        }

        if (LineSystemColumnCheckBox != null && LineSystemColumnComboBox != null)
        {
            LineSystemColumnComboBox.IsEnabled = LineSystemColumnCheckBox.IsChecked == true;
        }
    }

    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        if (NameColumnCheckBox.IsChecked == true && NameColumnComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("يجب تحديد عمود الاسم أو إلغاء تفعيل الخيار", "تحذير",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (NationalIdColumnCheckBox.IsChecked == true && NationalIdColumnComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("يجب تحديد عمود الرقم القومي أو إلغاء تفعيل الخيار", "تحذير",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (PhoneNumberColumnCheckBox.IsChecked == true && PhoneNumberColumnComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("يجب تحديد عمود رقم الخط أو إلغاء تفعيل الخيار", "تحذير",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (InternalIdColumnCheckBox.IsChecked == true && InternalIdColumnComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("يجب تحديد عمود الرقم الداخلي أو إلغاء تفعيل الخيار", "تحذير",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (HasCashWalletColumnCheckBox.IsChecked == true && HasCashWalletColumnComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("يجب تحديد عمود محفظة الكاش أو إلغاء تفعيل الخيار", "تحذير",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CashWalletNumberColumnCheckBox.IsChecked == true && CashWalletNumberColumnComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("يجب تحديد عمود رقم المحفظة أو إلغاء تفعيل الخيار", "تحذير",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (LineSystemColumnCheckBox.IsChecked == true && LineSystemColumnComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("يجب تحديد عمود نظام الخط أو إلغاء تفعيل الخيار", "تحذير",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Settings = new ImportSettings
        {
            HasHeader = HasHeaderCheckBox.IsChecked == true,
            NameColumn = NameColumnCheckBox.IsChecked == true && NameColumnComboBox.SelectedIndex != -1
                ? NameColumnComboBox.SelectedIndex + 1
                : null,
            NationalIdColumn = NationalIdColumnCheckBox.IsChecked == true && NationalIdColumnComboBox.SelectedIndex != -1
                ? NationalIdColumnComboBox.SelectedIndex + 1
                : null,
            PhoneNumberColumn = PhoneNumberColumnCheckBox.IsChecked == true && PhoneNumberColumnComboBox.SelectedIndex != -1
                ? PhoneNumberColumnComboBox.SelectedIndex + 1
                : null,
            InternalIdColumn = InternalIdColumnCheckBox.IsChecked == true && InternalIdColumnComboBox.SelectedIndex != -1
                ? InternalIdColumnComboBox.SelectedIndex + 1 
                : null,
            HasCashWalletColumn = HasCashWalletColumnCheckBox.IsChecked == true && HasCashWalletColumnComboBox.SelectedIndex != -1
                ? HasCashWalletColumnComboBox.SelectedIndex + 1 
                : null,
            CashWalletNumberColumn = CashWalletNumberColumnCheckBox.IsChecked == true && CashWalletNumberColumnComboBox.SelectedIndex != -1
                ? CashWalletNumberColumnComboBox.SelectedIndex + 1 
                : null,
            LineSystemColumn = LineSystemColumnCheckBox.IsChecked == true && LineSystemColumnComboBox.SelectedIndex != -1
                ? LineSystemColumnComboBox.SelectedIndex + 1 
                : null
        };

        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
