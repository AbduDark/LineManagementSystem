using System.Collections.ObjectModel;
using LineManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LineManagementSystem.Services;

public class AlertService
{
    private readonly DatabaseContext _context;
    private System.Timers.Timer? _alertTimer;

    public ObservableCollection<Alert> ActiveAlerts { get; } = new();

    public AlertService(DatabaseContext context)
    {
        _context = context;
        InitializeTimer();
    }

    private void InitializeTimer()
    {
        _alertTimer = new System.Timers.Timer(300000);
        _alertTimer.Elapsed += (s, e) => CheckForAlerts();
        _alertTimer.Start();
        
        CheckForAlerts();
    }

    public void CheckForAlerts()
    {
        try
        {
            var groups = _context.LineGroups
                .Include(g => g.Lines)
                .ToList();

            var newAlerts = new List<Alert>();

            foreach (var group in groups)
            {
                if (group.RequiresCashWallet && group.NeedsRenewalAlert())
                {
                    newAlerts.Add(new Alert
                    {
                        Type = AlertType.RenewalNeeded,
                        GroupId = group.Id,
                        Group = group,
                        Message = $"تحتاج المجموعة '{group.Name}' للتجديد"
                    });
                }

                if (group.RequiresCashWallet && group.IsRenewalExpired())
                {
                    newAlerts.Add(new Alert
                    {
                        Type = AlertType.RenewalExpired,
                        GroupId = group.Id,
                        Group = group,
                        Message = $"صلاحية المجموعة '{group.Name}' منتهية"
                    });
                }

                if (group.NeedsHandoverAlert())
                {
                    newAlerts.Add(new Alert
                    {
                        Type = AlertType.HandoverDue,
                        GroupId = group.Id,
                        Group = group,
                        Message = $"موعد تسليم المجموعة '{group.Name}' قريب"
                    });
                }

                if (group.IsHandoverOverdue())
                {
                    newAlerts.Add(new Alert
                    {
                        Type = AlertType.HandoverOverdue,
                        GroupId = group.Id,
                        Group = group,
                        Message = $"تأخر تسليم المجموعة '{group.Name}'"
                    });
                }
            }

            foreach (var alert in newAlerts)
            {
                _context.Alerts.Add(alert);
            }
            _context.SaveChanges();

            RefreshActiveAlerts();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطأ في فحص التنبيهات: {ex.Message}");
        }
    }

    public void RefreshActiveAlerts()
    {
        var alerts = _context.Alerts
            .Include(a => a.Group)
            .Where(a => !a.IsRead)
            .OrderByDescending(a => a.CreatedAt)
            .ToList();

        System.Windows.Application.Current?.Dispatcher.Invoke(() =>
        {
            ActiveAlerts.Clear();
            foreach (var alert in alerts)
            {
                ActiveAlerts.Add(alert);
            }
        });
    }

    public void MarkAsRead(int alertId)
    {
        var alert = _context.Alerts.Find(alertId);
        if (alert != null)
        {
            alert.IsRead = true;
            _context.SaveChanges();
            RefreshActiveAlerts();
        }
    }

    public void ClearAllAlerts()
    {
        var alerts = _context.Alerts.Where(a => !a.IsRead).ToList();
        foreach (var alert in alerts)
        {
            alert.IsRead = true;
        }
        _context.SaveChanges();
        RefreshActiveAlerts();
    }
}
