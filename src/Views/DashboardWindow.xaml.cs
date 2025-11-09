
using System.Windows;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;

namespace LineManagementSystem.Views;

public partial class DashboardWindow : Window
{
    private readonly DashboardViewModel _viewModel;

    public DashboardWindow(GroupService groupService, AlertService alertService)
    {
        InitializeComponent();
        _viewModel = new DashboardViewModel(groupService, alertService);
        DataContext = _viewModel;
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}
