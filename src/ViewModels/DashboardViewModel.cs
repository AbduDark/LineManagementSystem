
using System;
using System.Collections.Generic;
using System.Linq;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LineManagementSystem.Models;
using LineManagementSystem.Services;

namespace LineManagementSystem.ViewModels;

public class DashboardViewModel : BaseViewModel
{
    private readonly GroupService _groupService;
    private readonly AlertService _alertService;

    // Statistics Properties
    private int _totalGroups;
    public int TotalGroups
    {
        get => _totalGroups;
        set => SetProperty(ref _totalGroups, value);
    }

    private int _totalLines;
    public int TotalLines
    {
        get => _totalLines;
        set => SetProperty(ref _totalLines, value);
    }

    private int _activeGroups;
    public int ActiveGroups
    {
        get => _activeGroups;
        set => SetProperty(ref _activeGroups, value);
    }

    private int _pendingAlerts;
    public int PendingAlerts
    {
        get => _pendingAlerts;
        set => SetProperty(ref _pendingAlerts, value);
    }

    private DateTime _lastUpdateTime;
    public DateTime LastUpdateTime
    {
        get => _lastUpdateTime;
        set => SetProperty(ref _lastUpdateTime, value);
    }

    // Chart Series
    private IEnumerable<ISeries> _providerSeries = Array.Empty<ISeries>();
    public IEnumerable<ISeries> ProviderSeries
    {
        get => _providerSeries;
        set => SetProperty(ref _providerSeries, value);
    }

    private IEnumerable<ISeries> _statusSeries = Array.Empty<ISeries>();
    public IEnumerable<ISeries> StatusSeries
    {
        get => _statusSeries;
        set => SetProperty(ref _statusSeries, value);
    }

    private IEnumerable<ISeries> _walletSeries = Array.Empty<ISeries>();
    public IEnumerable<ISeries> WalletSeries
    {
        get => _walletSeries;
        set => SetProperty(ref _walletSeries, value);
    }

    private IEnumerable<ISeries> _renewalSeries = Array.Empty<ISeries>();
    public IEnumerable<ISeries> RenewalSeries
    {
        get => _renewalSeries;
        set => SetProperty(ref _renewalSeries, value);
    }

    public Axis[] XAxes { get; set; }
    public Axis[] MonthXAxes { get; set; }

    public DashboardViewModel(GroupService groupService, AlertService alertService)
    {
        _groupService = groupService;
        _alertService = alertService;

        XAxes = new[]
        {
            new Axis
            {
                Labels = new[] { "نشط", "معلق", "محظور" },
                LabelsRotation = 0,
                TextSize = 16,
                LabelsPaint = new SolidColorPaint(SKColors.Gray)
            }
        };

        MonthXAxes = new[]
        {
            new Axis
            {
                Labels = GetNext6Months(),
                LabelsRotation = 15,
                TextSize = 14,
                LabelsPaint = new SolidColorPaint(SKColors.Gray)
            }
        };

        LoadDashboardData();
    }

    private void LoadDashboardData()
    {
        // Get all groups
        var allGroups = new List<LineGroup>();
        allGroups.AddRange(_groupService.GetGroupsByProvider(TelecomProvider.Vodafone));
        allGroups.AddRange(_groupService.GetGroupsByProvider(TelecomProvider.Etisalat));
        allGroups.AddRange(_groupService.GetGroupsByProvider(TelecomProvider.We));
        allGroups.AddRange(_groupService.GetGroupsByProvider(TelecomProvider.Orange));

        // Calculate statistics
        TotalGroups = allGroups.Count;
        TotalLines = allGroups.Sum(g => g.GetLineCount);
        ActiveGroups = allGroups.Count(g => g.Status == GroupStatus.Active);
        PendingAlerts = _alertService.ActiveAlerts.Count;
        LastUpdateTime = DateTime.Now;

        // Provider Distribution
        var providerData = allGroups
            .GroupBy(g => g.Provider)
            .Select(g => new
            {
                Provider = g.Key,
                Count = g.Count()
            })
            .ToList();

        ProviderSeries = providerData.Select(p => new PieSeries<int>
        {
            Values = new[] { p.Count },
            Name = p.Provider.GetArabicName(),
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsSize = 16,
            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
            Fill = new SolidColorPaint(SKColor.Parse(p.Provider.GetColorHex()))
        }).ToArray();

        // Status Distribution
        var statusCounts = new[]
        {
            allGroups.Count(g => g.Status == GroupStatus.Active),
            allGroups.Count(g => g.Status == GroupStatus.Suspended),
            allGroups.Count(g => g.Status == GroupStatus.Barred)
        };

        StatusSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = statusCounts,
                Fill = new SolidColorPaint(SKColor.Parse("#4CAF50")),
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 16,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top
            }
        };

        // Wallet Distribution
        var allLines = allGroups.SelectMany(g => g.Lines).ToList();
        var withWallet = allLines.Count(l => l.HasCashWallet);
        var withoutWallet = allLines.Count - withWallet;

        WalletSeries = new ISeries[]
        {
            new PieSeries<int>
            {
                Values = new[] { withWallet },
                Name = "بمحفظة نقدية",
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 16,
                Fill = new SolidColorPaint(SKColor.Parse("#4CAF50"))
            },
            new PieSeries<int>
            {
                Values = new[] { withoutWallet },
                Name = "بدون محفظة",
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 16,
                Fill = new SolidColorPaint(SKColor.Parse("#F44336"))
            }
        };

        // Monthly Renewals
        var renewalCounts = GetMonthlyRenewalCounts(allGroups);
        RenewalSeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = renewalCounts,
                Fill = new SolidColorPaint(SKColor.Parse("#2196F3")),
                DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                DataLabelsSize = 14,
                DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top
            }
        };
    }

    private string[] GetNext6Months()
    {
        var months = new List<string>();
        for (int i = 0; i < 6; i++)
        {
            var date = DateTime.Now.AddMonths(i);
            months.Add($"{GetArabicMonth(date.Month)}\n{date.Year}");
        }
        return months.ToArray();
    }

    private string GetArabicMonth(int month)
    {
        return month switch
        {
            1 => "يناير",
            2 => "فبراير",
            3 => "مارس",
            4 => "أبريل",
            5 => "مايو",
            6 => "يونيو",
            7 => "يوليو",
            8 => "أغسطس",
            9 => "سبتمبر",
            10 => "أكتوبر",
            11 => "نوفمبر",
            12 => "ديسمبر",
            _ => ""
        };
    }

    private int[] GetMonthlyRenewalCounts(List<LineGroup> groups)
    {
        var counts = new int[6];
        var today = DateTime.Now;

        for (int i = 0; i < 6; i++)
        {
            var monthStart = new DateTime(today.Year, today.Month, 1).AddMonths(i);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            counts[i] = groups.Count(g =>
                g.NextRenewalDue.HasValue &&
                g.NextRenewalDue.Value >= monthStart &&
                g.NextRenewalDue.Value <= monthEnd);
        }

        return counts;
    }
}
