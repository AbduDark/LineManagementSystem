using System.Collections.ObjectModel;
using LineManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace LineManagementSystem.Services;

public class AlertService
{
    private System.Timers.Timer? _alertTimer;

    public ObservableCollection<Alert> ActiveAlerts { get; } = new();

    public AlertService()
    {
        InitializeTimer();
        CheckForAlerts();
    }

    private void InitializeTimer()
    {
        _alertTimer = new System.Timers.Timer(300000);
        _alertTimer.Elapsed += (s, e) => CheckForAlerts();
        _alertTimer.Start();
    }

    public void CheckForAlerts()
    {
        try
        {
            using var context = new DatabaseContext();
            
            var groups = context.LineGroups
                .Include(g => g.Lines)
                .ToList();

            foreach (var group in groups)
            {
                CheckAndCreateAlert(context, group, AlertType.RenewalNeeded, 
                    group.RequiresCashWallet && group.NeedsRenewalAlert());
                    
                CheckAndCreateAlert(context, group, AlertType.RenewalExpired, 
                    group.RequiresCashWallet && group.IsRenewalExpired());
                    
                CheckAndCreateAlert(context, group, AlertType.HandoverDue, 
                    group.NeedsHandoverAlert());
                    
                CheckAndCreateAlert(context, group, AlertType.HandoverOverdue, 
                    group.IsHandoverOverdue());
            }

            context.SaveChanges();
            RefreshActiveAlerts();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"خطأ في فحص التنبيهات: {ex.Message}");
        }
    }

    private void CheckAndCreateAlert(DatabaseContext context, LineGroup group, AlertType type, bool shouldAlert)
    {
        var existingAlert = context.Alerts
            .FirstOrDefault(a => a.GroupId == group.Id && a.Type == type && !a.IsRead);

        if (shouldAlert && existingAlert == null)
        {
            context.Alerts.Add(new Alert
            {
                Type = type,
                GroupId = group.Id,
                Message = GetAlertMessage(group, type),
                CreatedAt = DateTime.Now,
                IsRead = false
            });
        }
        else if (!shouldAlert && existingAlert != null)
        {
            existingAlert.IsRead = true;
        }
    }

    private string GetAlertMessage(LineGroup group, AlertType type)
    {
        return type switch
        {
            AlertType.RenewalNeeded => $"تحتاج المجموعة '{group.Name}' للتجديد",
            AlertType.RenewalExpired => $"صلاحية المجموعة '{group.Name}' منتهية",
            AlertType.HandoverDue => $"موعد تسليم المجموعة '{group.Name}' قريب",
            AlertType.HandoverOverdue => $"تأخر تسليم المجموعة '{group.Name}'",
            _ => ""
        };
    }

    public void RefreshActiveAlerts()
    {
        using var context = new DatabaseContext();
        var alerts = context.Alerts
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
        using var context = new DatabaseContext();
        var alert = context.Alerts.Find(alertId);
        if (alert != null)
        {
            alert.IsRead = true;
            context.SaveChanges();
            RefreshActiveAlerts();
        }
    }

    public void ClearAllAlerts()
    {
        using var context = new DatabaseContext();
        var alerts = context.Alerts.Where(a => !a.IsRead).ToList();
        foreach (var alert in alerts)
        {
            alert.IsRead = true;
        }
        context.SaveChanges();
        RefreshActiveAlerts();
    }
}
