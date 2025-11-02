using System.Windows;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;

namespace LineManagementSystem.Views;

public partial class SearchWindow : Window
{
    private readonly SearchViewModel _viewModel;
    private readonly GroupService _groupService;
    private readonly AlertService _alertService;

    public SearchWindow(GroupService groupService, AlertService alertService)
    {
        InitializeComponent();

        var context = new DatabaseContext();
        _viewModel = new SearchViewModel(groupService, context);
        _groupService = groupService;
        _alertService = alertService;

        _viewModel.NavigateToGroupDetails += OnNavigateToGroupDetails;
        _viewModel.EditLine += OnEditLine;
        _viewModel.DeleteLine += OnDeleteLine;
        _viewModel.DeleteGroup += OnDeleteGroup;

        DataContext = _viewModel;
    }

    private void OnNavigateToGroupDetails(LineGroup group)
    {
        var parentViewModel = new ProviderGroupsViewModel(_groupService, group.Provider);
        var detailsWindow = new GroupDetailsWindow(group, parentViewModel, _alertService);
        detailsWindow.Show();
    }

    private void OnEditLine(PhoneLine line, LineGroup group)
    {
        var parentViewModel = new ProviderGroupsViewModel(_groupService, group.Provider);
        var detailsWindow = new GroupDetailsWindow(group, parentViewModel, _alertService);
        detailsWindow.Show();
    }

    private void OnDeleteLine(int lineId)
    {
        try
        {
            _groupService.DeleteLine(lineId);
            System.Windows.MessageBox.Show("تم حذف الخط بنجاح", "نجح", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private void OnDeleteGroup(int groupId)
    {
        try
        {
            _groupService.DeleteGroup(groupId);
            System.Windows.MessageBox.Show("تم حذف المجموعة بنجاح", "نجح", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel.EditItemCommand.CanExecute(null))
        {
            var dataGrid = sender as System.Windows.Controls.DataGrid;
            if (dataGrid?.SelectedItem != null)
            {
                _viewModel.EditItemCommand.Execute(dataGrid.SelectedItem);
            }
        }
    }

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"نتائج_البحث_{DateTime.Now:yyyy-MM-dd}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var exportService = new ExportService();
                var groups = _viewModel.SearchResults.OfType<LineGroup>().ToList();
                
                if (groups.Any())
                {
                    exportService.ExportGroupsToExcel(groups, saveDialog.FileName);
                    MessageBox.Show("تم التصدير بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
                }
                else
                {
                    MessageBox.Show("لا توجد مجموعات للتصدير", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء التصدير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ExportPDF_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF Files|*.pdf",
                FileName = $"نتائج_البحث_{DateTime.Now:yyyy-MM-dd}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var exportService = new ExportService();
                var groups = _viewModel.SearchResults.OfType<LineGroup>().ToList();
                
                if (groups.Any())
                {
                    exportService.ExportGroupsToPDF(groups, saveDialog.FileName);
                    MessageBox.Show("تم التصدير بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                    System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
                }
                else
                {
                    MessageBox.Show("لا توجد مجموعات للتصدير", "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء التصدير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}