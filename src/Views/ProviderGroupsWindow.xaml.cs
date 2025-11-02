using System.Windows;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;

namespace LineManagementSystem.Views;

public partial class ProviderGroupsWindow : Window
{
    private readonly ProviderGroupsViewModel _viewModel;
    private readonly AlertService _alertService;

    public ProviderGroupsWindow(TelecomProvider provider, GroupService groupService, AlertService alertService)
    {
        InitializeComponent();

        _alertService = alertService;
        _viewModel = new ProviderGroupsViewModel(groupService, provider);
        _viewModel.NavigateToGroupDetails += OnNavigateToGroupDetails;
        _viewModel.OpenGroupDialog += OnOpenGroupDialog;

        DataContext = _viewModel;
    }

    private void OnNavigateToGroupDetails(LineGroup group)
    {
        var detailsWindow = new GroupDetailsWindow(group, _viewModel, _alertService);
        detailsWindow.Show();
    }

    private void OnOpenGroupDialog(LineGroup? group)
    {
        var dialog = new GroupDialog(group, _viewModel.Provider);
        if (dialog.ShowDialog() == true && dialog.ResultGroup != null)
        {
            _viewModel.SaveGroup(dialog.ResultGroup);
        }
    }

    private void DataGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && _viewModel.SelectedGroup != null)
        {
            if (_viewModel.ViewGroupDetailsCommand.CanExecute(null))
            {
                _viewModel.ViewGroupDetailsCommand.Execute(null);
            }
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel Files|*.xlsx",
                FileName = $"مجموعات_{_viewModel.Provider.GetArabicName()}_{DateTime.Now:yyyy-MM-dd}.xlsx"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var exportService = new ExportService();
                exportService.ExportGroupsToExcel(_viewModel.Groups, saveDialog.FileName);
                MessageBox.Show("تم التصدير بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
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
                FileName = $"مجموعات_{_viewModel.Provider.GetArabicName()}_{DateTime.Now:yyyy-MM-dd}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var exportService = new ExportService();
                exportService.ExportGroupsToPDF(_viewModel.Groups, saveDialog.FileName);
                MessageBox.Show("تم التصدير بنجاح!", "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
                System.Diagnostics.Process.Start("explorer.exe", $"/select,\"{saveDialog.FileName}\"");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حدث خطأ أثناء التصدير: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedGroup != null)
        {
            if (_viewModel.EditGroupCommand.CanExecute(null))
            {
                _viewModel.EditGroupCommand.Execute(null);
            }
        }
    }
}