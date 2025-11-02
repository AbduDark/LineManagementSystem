
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
