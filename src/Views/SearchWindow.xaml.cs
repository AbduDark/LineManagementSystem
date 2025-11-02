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
        MessageBox.Show(
            "قريباً: تصدير نتائج البحث إلى Excel\n\nسيتم تصدير جميع النتائج المعروضة بتنسيق احترافي",
            "ميزة قادمة",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ExportPDF_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "قريباً: تصدير نتائج البحث إلى PDF\n\nسيتم إنشاء تقرير PDF بتنسيق احترافي",
            "ميزة قادمة",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}