using System.Collections.ObjectModel;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;

namespace LineManagementSystem.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly AlertService _alertService;
    private readonly GroupService _groupService;

    public ObservableCollection<Alert> Alerts => _alertService.ActiveAlerts;

    public ICommand NavigateToProviderCommand { get; }
    public ICommand ClearAlertsCommand { get; }
    public ICommand MarkAlertReadCommand { get; }

    public event Action<TelecomProvider>? NavigateToProvider;

    public MainViewModel(AlertService alertService, GroupService groupService)
    {
        _alertService = alertService;
        _groupService = groupService;

        NavigateToProviderCommand = new RelayCommand<TelecomProvider>(provider =>
        {
            NavigateToProvider?.Invoke(provider);
        });

        ClearAlertsCommand = new RelayCommand(() =>
        {
            _alertService.ClearAllAlerts();
        });

        MarkAlertReadCommand = new RelayCommand<int>(alertId =>
        {
            _alertService.MarkAsRead(alertId);
        });

        _alertService.RefreshActiveAlerts();
    }

    public Dictionary<TelecomProvider, int> GetProviderCounts()
    {
        return _groupService.GetGroupCountsByProvider();
    }
}
