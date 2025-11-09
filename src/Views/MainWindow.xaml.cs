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

    private void VodafoneButton_Click(object sender, RoutedEventArgs e)
    {
        OnNavigateToProvider(TelecomProvider.Vodafone);
    }

    private void EtisalatButton_Click(object sender, RoutedEventArgs e)
    {
        OnNavigateToProvider(TelecomProvider.Etisalat);
    }

    private void WeButton_Click(object sender, RoutedEventArgs e)
    {
        OnNavigateToProvider(TelecomProvider.We);
    }

    private void OrangeButton_Click(object sender, RoutedEventArgs e)
    {
        OnNavigateToProvider(TelecomProvider.Orange);
    }

    private void DashboardButton_Click(object sender, RoutedEventArgs e)
    {
        var dashboardWindow = new DashboardWindow(_groupService, _alertService);
        dashboardWindow.Show();
    }

    private void SearchButton_Click(object sender, RoutedEventArgs e)
    {
        var searchWindow = new SearchWindow(_groupService, _alertService);
        searchWindow.Show();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.ShowDialog();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Application.Current.Shutdown();
    }
}
