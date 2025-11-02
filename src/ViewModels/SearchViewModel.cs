
using System.Collections.ObjectModel;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using Microsoft.EntityFrameworkCore;

namespace LineManagementSystem.ViewModels;

public class SearchViewModel : BaseViewModel
{
    private readonly GroupService _groupService;
    private readonly DatabaseContext _context;
    private string _searchQuery = string.Empty;
    private string _searchType = "الكل";

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            SetProperty(ref _searchQuery, value);
            SearchCommand.Execute(null);
        }
    }

    public string SearchType
    {
        get => _searchType;
        set
        {
            SetProperty(ref _searchType, value);
            SearchCommand.Execute(null);
        }
    }

    public ObservableCollection<SearchResult> SearchResults { get; } = new();

    public ICommand SearchCommand { get; }
    public ICommand ClearCommand { get; }
    public ICommand OpenItemCommand { get; }
    public ICommand DeleteItemCommand { get; }
    public ICommand EditItemCommand { get; }

    public event Action<LineGroup>? NavigateToGroupDetails;
    public event Action<PhoneLine, LineGroup>? EditLine;
    public event Action<int>? DeleteLine;
    public event Action<int>? DeleteGroup;

    public SearchViewModel(GroupService groupService, DatabaseContext context)
    {
        _groupService = groupService;
        _context = context;

        SearchCommand = new RelayCommand(() =>
        {
            PerformSearch();
        });

        ClearCommand = new RelayCommand(() =>
        {
            SearchQuery = string.Empty;
            SearchResults.Clear();
        });

        OpenItemCommand = new RelayCommand<SearchResult>((result) =>
        {
            if (result == null) return;

            if (result.Type == "مجموعة")
            {
                // فتح تفاصيل المجموعة
                var group = _context.LineGroups
                    .Include(g => g.Lines)
                    .FirstOrDefault(g => g.Name == result.GroupName);
                
                if (group != null)
                {
                    NavigateToGroupDetails?.Invoke(group);
                }
            }
            else if (result.Type == "خط" || result.Type == "خط (من المجموعة)")
            {
                // فتح تعديل الخط
                var line = _context.PhoneLines
                    .Include(l => l.Group)
                    .FirstOrDefault(l => l.PhoneNumber == result.PhoneNumber && l.NationalId == result.NationalId);
                
                if (line != null && line.Group != null)
                {
                    EditLine?.Invoke(line, line.Group);
                }
            }
        });

        DeleteItemCommand = new RelayCommand<SearchResult>((result) =>
        {
            if (result == null) return;

            var confirmResult = System.Windows.MessageBox.Show(
                $"هل أنت متأكد من حذف {result.Type}: {(result.Type == "مجموعة" ? result.GroupName : result.Name)}؟",
                "تأكيد الحذف",
                System.Windows.MessageBoxButton.YesNo,
                System.Windows.MessageBoxImage.Warning);

            if (confirmResult == System.Windows.MessageBoxResult.Yes)
            {
                // تنظيف الـ tracker قبل الحذف
                _context.ChangeTracker.Clear();
                
                if (result.Type == "مجموعة")
                {
                    var group = _context.LineGroups.FirstOrDefault(g => g.Name == result.GroupName);
                    if (group != null)
                    {
                        DeleteGroup?.Invoke(group.Id);
                        PerformSearch();
                    }
                }
                else if (result.Type == "خط" || result.Type == "خط (من المجموعة)")
                {
                    var line = _context.PhoneLines
                        .FirstOrDefault(l => l.PhoneNumber == result.PhoneNumber && l.NationalId == result.NationalId);
                    if (line != null)
                    {
                        DeleteLine?.Invoke(line.Id);
                        PerformSearch();
                    }
                }
            }
        });

        EditItemCommand = new RelayCommand<SearchResult>((result) =>
        {
            if (result == null) return;

            if (result.Type == "مجموعة")
            {
                var group = _context.LineGroups
                    .Include(g => g.Lines)
                    .FirstOrDefault(g => g.Name == result.GroupName);
                
                if (group != null)
                {
                    NavigateToGroupDetails?.Invoke(group);
                }
            }
            else if (result.Type == "خط" || result.Type == "خط (من المجموعة)")
            {
                var line = _context.PhoneLines
                    .Include(l => l.Group)
                    .FirstOrDefault(l => l.PhoneNumber == result.PhoneNumber && l.NationalId == result.NationalId);
                
                if (line != null && line.Group != null)
                {
                    EditLine?.Invoke(line, line.Group);
                }
            }
        });
    }

    private void PerformSearch()
    {
        SearchResults.Clear();

        if (string.IsNullOrWhiteSpace(SearchQuery))
            return;

        var query = SearchQuery.Trim().ToLower();

        // البحث في الخطوط
        var lines = _context.PhoneLines
            .Include(l => l.Group)
            .Where(l =>
                (SearchType == "الكل" || SearchType == "رقم قومي") && l.NationalId.ToLower().Contains(query) ||
                (SearchType == "الكل" || SearchType == "اسم الشخص") && l.Name.ToLower().Contains(query) ||
                (SearchType == "الكل" || SearchType == "رقم الخط") && l.PhoneNumber.ToLower().Contains(query) ||
                (SearchType == "الكل" || SearchType == "رقم داخلي") && l.InternalId.ToLower().Contains(query) ||
                (SearchType == "الكل" || SearchType == "محفظة كاش") && (l.CashWalletNumber ?? "").ToLower().Contains(query)
            )
            .ToList();

        foreach (var line in lines)
        {
            SearchResults.Add(new SearchResult
            {
                Type = "خط",
                Name = line.Name,
                PhoneNumber = line.PhoneNumber,
                NationalId = line.NationalId,
                InternalId = line.InternalId,
                GroupName = line.Group?.Name ?? "غير محدد",
                Provider = line.Group?.Provider.GetArabicName() ?? "غير محدد",
                CashWalletNumber = line.CashWalletNumber,
                HasCashWallet = line.HasCashWallet,
                Details = line.Details
            });
        }

        // البحث في المجموعات
        if (SearchType == "الكل" || SearchType == "اسم المجموعة")
        {
            var groups = _context.LineGroups
                .Include(g => g.Lines)
                .Where(g => g.Name.ToLower().Contains(query))
                .ToList();

            foreach (var group in groups)
            {
                // إضافة المجموعة نفسها
                SearchResults.Add(new SearchResult
                {
                    Type = "مجموعة",
                    GroupName = group.Name,
                    Provider = group.Provider.GetArabicName(),
                    LineCount = group.GetLineCount,
                    Details = $"عدد الخطوط: {group.GetLineCount} من {group.MaxLines}"
                });

                // إضافة كل الخطوط في المجموعة
                foreach (var line in group.Lines)
                {
                    SearchResults.Add(new SearchResult
                    {
                        Type = "خط (من المجموعة)",
                        Name = line.Name,
                        PhoneNumber = line.PhoneNumber,
                        NationalId = line.NationalId,
                        InternalId = line.InternalId,
                        GroupName = group.Name,
                        Provider = group.Provider.GetArabicName(),
                        CashWalletNumber = line.CashWalletNumber,
                        HasCashWallet = line.HasCashWallet,
                        Details = line.Details
                    });
                }
            }
        }
    }
}

public class SearchResult
{
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string InternalId { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string? CashWalletNumber { get; set; }
    public bool HasCashWallet { get; set; }
    public string? Details { get; set; }
    public int LineCount { get; set; }
}
