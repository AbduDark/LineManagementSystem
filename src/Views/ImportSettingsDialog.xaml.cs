using System.Windows;
using System.Windows.Controls;
using ClosedXML.Excel;

namespace LineManagementSystem.Views;

public partial class ImportSettingsDialog : Window
{
    public class ImportSettings
    {
        public bool HasHeader { get; set; }
        public int NameColumn { get; set; }
        public int NationalIdColumn { get; set; }
        public int PhoneNumberColumn { get; set; }
        public int? InternalIdColumn { get; set; }
        public int? HasCashWalletColumn { get; set; }
        public int? CashWalletNumberColumn { get; set; }
    }

    public ImportSettings? Settings { get; private set; }
    private readonly string _excelFilePath;

    public ImportSettingsDialog(string excelFilePath)
    {
        InitializeComponent();
        _excelFilePath = excelFilePath;
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

            NameColumnComboBox.ItemsSource = columns;
            NationalIdColumnComboBox.ItemsSource = columns;
            PhoneNumberColumnComboBox.ItemsSource = columns;
            InternalIdColumnComboBox.ItemsSource = columns;
            HasCashWalletColumnComboBox.ItemsSource = columns;
            CashWalletNumberColumnComboBox.ItemsSource = columns;

            NameColumnComboBox.SelectedIndex = 0;
            if (columns.Count > 1) NationalIdColumnComboBox.SelectedIndex = 1;
            if (columns.Count > 2) PhoneNumberColumnComboBox.SelectedIndex = 2;
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
    }

    private void ImportButton_Click(object sender, RoutedEventArgs e)
    {
        if (NameColumnComboBox.SelectedIndex == -1 ||
            NationalIdColumnComboBox.SelectedIndex == -1 ||
            PhoneNumberColumnComboBox.SelectedIndex == -1)
        {
            MessageBox.Show("يجب تحديد الأعمدة الإلزامية (الاسم، الرقم القومي، رقم الخط)", "تحذير",
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

        Settings = new ImportSettings
        {
            HasHeader = HasHeaderCheckBox.IsChecked == true,
            NameColumn = NameColumnComboBox.SelectedIndex + 1,
            NationalIdColumn = NationalIdColumnComboBox.SelectedIndex + 1,
            PhoneNumberColumn = PhoneNumberColumnComboBox.SelectedIndex + 1,
            InternalIdColumn = InternalIdColumnCheckBox.IsChecked == true 
                ? InternalIdColumnComboBox.SelectedIndex + 1 
                : null,
            HasCashWalletColumn = HasCashWalletColumnCheckBox.IsChecked == true 
                ? HasCashWalletColumnComboBox.SelectedIndex + 1 
                : null,
            CashWalletNumberColumn = CashWalletNumberColumnCheckBox.IsChecked == true 
                ? CashWalletNumberColumnComboBox.SelectedIndex + 1 
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
