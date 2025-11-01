using System.Windows;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;

namespace LineManagementSystem.Views;

public partial class MainWindow : Window
{
    private readonly DatabaseContext _dbContext;
    private readonly AlertService _alertService;
    private readonly GroupService _groupService;
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        _dbContext = new DatabaseContext();
        _dbContext.EnsureCreated();

        _alertService = new AlertService();
        _groupService = new GroupService(_dbContext);

        _viewModel = new MainViewModel(_alertService, _groupService);
        _viewModel.NavigateToProvider += OnNavigateToProvider;

        DataContext = _viewModel;
    }

    private void OnNavigateToProvider(TelecomProvider provider)
    {
        var groupsWindow = new ProviderGroupsWindow(provider, _groupService, _alertService);
        groupsWindow.Show();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Application.Current.Shutdown();
    }
}
