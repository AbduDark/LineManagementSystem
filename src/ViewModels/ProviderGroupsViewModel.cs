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
    private string _searchQuery = string.Empty;
    private List<LineGroup> _allGroups = new();

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

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (SetProperty(ref _searchQuery, value))
            {
                FilterGroups();
            }
        }
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
                var groupName = SelectedGroup.Name;
                var lineCount = SelectedGroup.Lines?.Count ?? 0;
                var message = $"⚠️ تحذير: حذف نهائي\n\n" +
                             $"المجموعة: {groupName}\n" +
                             $"عدد الخطوط: {lineCount}\n" +
                             $"الشركة: {SelectedGroup.Provider}\n\n" +
                             $"سيتم حذف المجموعة وجميع الخطوط التابعة لها ({lineCount} خط) بشكل نهائي.\n" +
                             $"هذا الإجراء لا يمكن التراجع عنه!\n\n" +
                             $"هل أنت متأكد من رغبتك في المتابعة؟";

                var confirmResult = System.Windows.MessageBox.Show(
                    message,
                    "⚠️ تأكيد الحذف النهائي",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning,
                    System.Windows.MessageBoxResult.No);

                if (confirmResult == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        _groupService.DeleteGroup(SelectedGroup.Id);
                        LoadGroups();
                        System.Windows.MessageBox.Show(
                            $"✓ تم حذف المجموعة '{groupName}' بنجاح",
                            "نجح الحذف",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(
                            $"❌ فشل الحذف\n\nحدث خطأ أثناء حذف المجموعة:\n{ex.Message}\n\nالرجاء المحاولة مرة أخرى أو الاتصال بالدعم الفني.",
                            "خطأ في الحذف",
                            System.Windows.MessageBoxButton.OK,
                            System.Windows.MessageBoxImage.Error);
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
        _allGroups = _groupService.GetGroupsByProvider(Provider);
        FilterGroups();
    }

    private void FilterGroups()
    {
        Groups.Clear();
        
        var filteredGroups = _allGroups;
        
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var query = SearchQuery.Trim().ToLower();
            filteredGroups = _allGroups.Where(g =>
                g.Name.ToLower().Contains(query) ||
                (g.AssignedToEmployee?.ToLower().Contains(query) ?? false) ||
                (g.AssignedCustomer?.ToLower().Contains(query) ?? false) ||
                (g.AdditionalDetails?.ToLower().Contains(query) ?? false)
            ).ToList();
        }
        
        foreach (var group in filteredGroups)
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