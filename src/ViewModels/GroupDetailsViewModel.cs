using System.Collections.ObjectModel;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;

namespace LineManagementSystem.ViewModels;

public class GroupDetailsViewModel : BaseViewModel
{
    private readonly GroupService _groupService;
    private LineGroup _group;
    private PhoneLine? _selectedLine;
    private bool _isAddingLine;

    public LineGroup Group
    {
        get => _group;
        set => SetProperty(ref _group, value);
    }

    public ObservableCollection<PhoneLine> Lines { get; } = new();

    public PhoneLine? SelectedLine
    {
        get => _selectedLine;
        set => SetProperty(ref _selectedLine, value);
    }

    public bool IsAddingLine
    {
        get => _isAddingLine;
        set => SetProperty(ref _isAddingLine, value);
    }

    public ICommand AddLineCommand { get; }
    public ICommand EditLineCommand { get; }
    public ICommand DeleteLineCommand { get; }
    public ICommand SaveLineCommand { get; }
    public ICommand CancelAddLineCommand { get; }
    public ICommand SelectAllCashWalletCommand { get; }

    private PhoneLine _newLine = new();
    public PhoneLine NewLine
    {
        get => _newLine;
        set => SetProperty(ref _newLine, value);
    }

    public event Action? LineAdded;

    public GroupDetailsViewModel(GroupService groupService, LineGroup group)
    {
        _groupService = groupService;
        _group = group;

        AddLineCommand = new RelayCommand(() =>
        {
            if (Group.CanAddMoreLines)
            {
                NewLine = new PhoneLine();
                IsAddingLine = true;
            }
        }, () => Group.CanAddMoreLines);

        EditLineCommand = new RelayCommand(() =>
        {
            if (SelectedLine != null)
            {
                NewLine = new PhoneLine
                {
                    Id = SelectedLine.Id,
                    Name = SelectedLine.Name,
                    NationalId = SelectedLine.NationalId,
                    PhoneNumber = SelectedLine.PhoneNumber,
                    InternalId = SelectedLine.InternalId,
                    HasCashWallet = SelectedLine.HasCashWallet,
                    CashWalletNumber = SelectedLine.CashWalletNumber,
                    GroupId = SelectedLine.GroupId
                };
                IsAddingLine = true;
            }
        }, () => SelectedLine != null);

        DeleteLineCommand = new RelayCommand(() =>
        {
            if (SelectedLine != null)
            {
                var confirmResult = System.Windows.MessageBox.Show(
                    $"هل تريد حذف الخط: {SelectedLine.PhoneNumber}؟",
                    "تأكيد الحذف",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);

                if (confirmResult == System.Windows.MessageBoxResult.Yes)
                {
                    try
                    {
                        _groupService.DeleteLine(SelectedLine.Id);
                        LoadLines();
                        SelectedLine = null;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show($"حدث خطأ أثناء الحذف: {ex.Message}", "خطأ", 
                            System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    }
                }
            }
        }, () => SelectedLine != null);

        SaveLineCommand = new RelayCommand(() =>
        {
            try
            {
                if (NewLine.Id == 0)
                {
                    _groupService.AddLineToGroup(Group.Id, NewLine);
                }
                else
                {
                    _groupService.UpdateLine(NewLine);
                }
                LoadLines();
                IsAddingLine = false;
                LineAdded?.Invoke();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "خطأ", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        });

        CancelAddLineCommand = new RelayCommand(() =>
        {
            IsAddingLine = false;
            NewLine = new PhoneLine();
        });

        SelectAllCashWalletCommand = new RelayCommand(() =>
        {
            foreach (var line in Lines)
            {
                line.HasCashWallet = true;
            }
            
            // حفظ التغييرات في قاعدة البيانات
            foreach (var line in Lines)
            {
                _groupService.UpdateLine(line);
            }
            
            LoadLines();
        }, () => Lines.Any());

        LoadLines();
    }

    public void LoadLines()
    {
        var group = _groupService.GetGroupById(Group.Id);
        if (group != null)
        {
            Group = group;
            Lines.Clear();
            foreach (var line in group.Lines)
            {
                Lines.Add(line);
            }
        }
    }
}
