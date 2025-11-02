using System.Collections.ObjectModel;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;

namespace LineManagementSystem.ViewModels;

public class ProviderGroupsViewModel : BaseViewModel
{
    private readonly GroupService _groupService;
    private TelecomProvider _provider;
    private LineGroup? _selectedGroup;

    public TelecomProvider Provider
    {
        get => _provider;
        set => SetProperty(ref _provider, value);
    }

    public ObservableCollection<LineGroup> Groups { get; } = new();

    public LineGroup? SelectedGroup
    {
        get => _selectedGroup;
        set => SetProperty(ref _selectedGroup, value);
    }

    public ICommand AddGroupCommand { get; }
    public ICommand EditGroupCommand { get; }
    public ICommand DeleteGroupCommand { get; }
    public ICommand RenewLicenseCommand { get; }
    public ICommand ViewGroupDetailsCommand { get; }

    public event Action<LineGroup>? NavigateToGroupDetails;
    public event Action<LineGroup?>? OpenGroupDialog;

    public ProviderGroupsViewModel(GroupService groupService, TelecomProvider provider)
    {
        _groupService = groupService;
        _provider = provider;

        AddGroupCommand = new RelayCommand(() =>
        {
            OpenGroupDialog?.Invoke(null);
        });

        EditGroupCommand = new RelayCommand(() =>
        {
            if (SelectedGroup != null)
                OpenGroupDialog?.Invoke(SelectedGroup);
        }, () => SelectedGroup != null);

        DeleteGroupCommand = new RelayCommand(() =>
        {
            if (SelectedGroup != null)
            {
                var confirmResult = System.Windows.MessageBox.Show(
                    "هل تريد حذف هذه المجموعة وكل الخطوط التابعة لها؟",
                    "تأكيد الحذف",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (confirmResult == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        _groupService.DeleteGroup(SelectedGroup.Id);
                        LoadGroups();
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ",
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }, () => SelectedGroup != null);

        RenewLicenseCommand = new RelayCommand(() =>
        {
            if (SelectedGroup != null)
            {
                _groupService.RenewGroupLicense(SelectedGroup.Id);
                LoadGroups();
            }
        }, () => SelectedGroup != null && SelectedGroup.RequiresCashWallet);

        ViewGroupDetailsCommand = new RelayCommand(() =>
        {
            if (SelectedGroup != null)
                NavigateToGroupDetails?.Invoke(SelectedGroup);
        }, () => SelectedGroup != null);

        LoadGroups();
    }

    public void LoadGroups()
    {
        var groups = _groupService.GetGroupsByProvider(Provider);
        Groups.Clear();
        foreach (var group in groups)
        {
            Groups.Add(group);
        }
    }

    public void SaveGroup(LineGroup group)
    {
        if (group.Id == 0)
        {
            group.Provider = Provider;
            _groupService.CreateGroup(group);
        }
        else
        {
            _groupService.UpdateGroup(group);
        }
        LoadGroups();
    }
}