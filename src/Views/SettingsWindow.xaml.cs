using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using LineManagementSystem.Services;

namespace LineManagementSystem.Views;

public partial class SettingsWindow : Window
{
    private readonly BackupService _backupService;

    public bool IsDarkMode
    {
        get => ThemeManager.IsDarkMode;
        set
        {
            ThemeManager.SetTheme(value);
            UpdateThemeText();
        }
    }

    public SettingsWindow()
    {
        InitializeComponent();
        DataContext = this;
        _backupService = new BackupService();
        UpdateThemeText();
        UpdateBackupStatus();
        
        AutoBackupToggle.IsChecked = _backupService.IsAutoBackupEnabled;
    }

    private void UpdateThemeText()
    {
        if (CurrentThemeText != null)
        {
            var themeMode = ThemeManager.IsDarkMode ? "الوضع الليلي نشط" : "الوضع النهاري نشط";
            CurrentThemeText.Text = $"الوضع الحالي: {themeMode}";
        }
    }

    private void UpdateBackupStatus()
    {
        if (BackupStatusText != null)
        {
            var backups = _backupService.GetBackupList();
            var status = _backupService.IsAutoBackupEnabled 
                ? $"✓ النسخ التلقائي مفعّل - عدد النسخ المتاحة: {backups.Length}" 
                : $"عدد النسخ المتاحة: {backups.Length}";
            BackupStatusText.Text = status;
        }
    }

    private void DarkModeToggle_Changed(object sender, RoutedEventArgs e)
    {
        if (DarkModeToggle != null)
        {
            ThemeManager.SetTheme(DarkModeToggle.IsChecked == true);
            UpdateThemeText();
        }
    }

    private async void CreateBackup_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Database files (*.db)|*.db",
                DefaultExt = "db",
                FileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.db",
                InitialDirectory = _backupService.BackupDirectory
            };

            if (dialog.ShowDialog() == true)
            {
                var customName = Path.GetFileNameWithoutExtension(dialog.FileName);
                var backupPath = await _backupService.CreateBackup(customName);
                
                File.Copy(backupPath, dialog.FileName, overwrite: true);
                
                UpdateBackupStatus();
                MessageBox.Show($"تم إنشاء النسخة الاحتياطية بنجاح!\n\nالمسار:\n{dialog.FileName}", 
                    "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في إنشاء النسخة الاحتياطية:\n{ex.Message}", 
                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void RestoreBackup_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show(
            "هل أنت متأكد من استعادة البيانات من نسخة احتياطية؟\n\n⚠️ سيتم استبدال جميع البيانات الحالية!\n\nيُنصح بإنشاء نسخة احتياطية أولاً.",
            "تأكيد الاستعادة",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Database files (*.db)|*.db",
                DefaultExt = "db",
                InitialDirectory = _backupService.BackupDirectory,
                Title = "اختر النسخة الاحتياطية للاستعادة"
            };

            if (dialog.ShowDialog() == true)
            {
                await _backupService.RestoreBackup(dialog.FileName);
                
                MessageBox.Show(
                    "تمت استعادة البيانات بنجاح!\n\nسيتم إعادة تشغيل التطبيق الآن.",
                    "نجح",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                Application.Current.Shutdown();
                Process.Start(Process.GetCurrentProcess().MainModule!.FileName!);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في استعادة النسخة الاحتياطية:\n{ex.Message}", 
                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OpenBackupFolder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (!Directory.Exists(_backupService.BackupDirectory))
            {
                Directory.CreateDirectory(_backupService.BackupDirectory);
            }
            
            Process.Start("explorer.exe", _backupService.BackupDirectory);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في فتح المجلد:\n{ex.Message}", 
                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void DeleteOldBackups_Click(object sender, RoutedEventArgs e)
    {
        var backups = _backupService.GetBackupList();
        
        if (backups.Length == 0)
        {
            MessageBox.Show("لا توجد نسخ احتياطية للحذف", 
                "معلومات", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var result = MessageBox.Show(
            $"عدد النسخ الاحتياطية: {backups.Length}\n\nهل تريد حذف جميع النسخ القديمة والاحتفاظ بآخر 10 نسخ فقط؟",
            "تأكيد الحذف",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                var toDelete = backups.Skip(10).ToArray();
                foreach (var backup in toDelete)
                {
                    _backupService.DeleteBackup(backup);
                }
                
                UpdateBackupStatus();
                MessageBox.Show($"تم حذف {toDelete.Length} نسخة احتياطية قديمة", 
                    "نجح", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حذف النسخ القديمة:\n{ex.Message}", 
                    "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void AutoBackup_Changed(object sender, RoutedEventArgs e)
    {
        if (AutoBackupToggle != null)
        {
            if (AutoBackupToggle.IsChecked == true)
            {
                _backupService.StartAutoBackup(24);
            }
            else
            {
                _backupService.StopAutoBackup();
            }
            UpdateBackupStatus();
        }
    }

    private async void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "قريباً: تصدير تقرير شامل بصيغة Excel\n\nالتقرير سيتضمن:\n• جميع المجموعات والخطوط\n• التنبيهات النشطة\n• إحصائيات الشركات\n• تفاصيل التجديد والتسليم",
            "ميزة قادمة",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        
        await System.Threading.Tasks.Task.CompletedTask;
    }

    private async void ExportPDF_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            "قريباً: تصدير تقرير بصيغة PDF\n\nالتقرير سيتضمن:\n• ملخص شامل للبيانات\n• رسوم بيانية وإحصائيات\n• قوائم مفصلة بتنسيق احترافي",
            "ميزة قادمة",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
        
        await System.Threading.Tasks.Task.CompletedTask;
    }

    private async void ProvidersStats_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            using var db = new DatabaseContext();
            var stats = db.LineGroups
                .GroupBy(g => g.Provider)
                .Select(g => new
                {
                    Provider = g.Key,
                    Groups = g.Count(),
                    Lines = g.Sum(x => x.Lines.Count),
                    WithWallet = g.Count(x => x.RequiresCashWallet)
                })
                .ToList();

            var message = "📊 إحصائيات الشركات:\n\n";
            foreach (var stat in stats)
            {
                message += $"🔹 {stat.Provider}:\n";
                message += $"   • المجموعات: {stat.Groups}\n";
                message += $"   • الخطوط: {stat.Lines}\n";
                message += $"   • مع محفظة: {stat.WithWallet}\n\n";
            }

            var totalGroups = stats.Sum(s => s.Groups);
            var totalLines = stats.Sum(s => s.Lines);
            message += $"━━━━━━━━━━━━━━━━\n";
            message += $"المجموع الكلي:\n";
            message += $"• {totalGroups} مجموعة\n";
            message += $"• {totalLines} خط";

            MessageBox.Show(message, "الإحصائيات", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في جلب الإحصائيات:\n{ex.Message}", 
                "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        await System.Threading.Tasks.Task.CompletedTask;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
    }
}
