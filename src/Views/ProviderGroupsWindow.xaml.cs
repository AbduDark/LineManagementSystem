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
}
