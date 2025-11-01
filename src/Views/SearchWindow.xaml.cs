
using System.Windows;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;

namespace LineManagementSystem.Views;

public partial class SearchWindow : Window
{
    private readonly SearchViewModel _viewModel;

    public SearchWindow(GroupService groupService)
    {
        InitializeComponent();

        var context = new DatabaseContext();
        _viewModel = new SearchViewModel(groupService, context);
        DataContext = _viewModel;
    }
}
