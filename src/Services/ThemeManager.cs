using System.Windows;
using System.Windows.Media;

namespace LineManagementSystem.Services;

public static class ThemeManager
{
    public static bool IsDarkMode { get; private set; } = false;

    public static void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        ApplyTheme();
    }

    public static void SetTheme(bool isDark)
    {
        IsDarkMode = isDark;
        ApplyTheme();
    }

    private static void ApplyTheme()
    {
        var app = Application.Current;
        if (app?.Resources == null) return;

        if (IsDarkMode)
        {
            app.Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(18, 18, 18));
            app.Resources["CardBackground"] = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            app.Resources["HeaderBackground"] = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            app.Resources["TextPrimary"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            app.Resources["TextSecondary"] = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            app.Resources["BorderColor"] = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            app.Resources["HoverBackground"] = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            app.Resources["DataGridBackground"] = new SolidColorBrush(Color.FromRgb(25, 25, 25));
            app.Resources["DataGridHeaderBackground"] = new SolidColorBrush(Color.FromRgb(45, 45, 45));
            app.Resources["DataGridHeaderForeground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            app.Resources["DataGridAlternateRow"] = new SolidColorBrush(Color.FromRgb(30, 30, 30));
            app.Resources["DataGridRowBackground"] = new SolidColorBrush(Color.FromRgb(28, 28, 28));
            app.Resources["DataGridRowHover"] = new SolidColorBrush(Color.FromRgb(45, 45, 45));
            app.Resources["DataGridRowSelected"] = new SolidColorBrush(Color.FromRgb(25, 118, 210));
            app.Resources["DataGridCellSelected"] = new SolidColorBrush(Color.FromRgb(21, 101, 192));
            app.Resources["DataGridGridLines"] = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            app.Resources["InputBackground"] = new SolidColorBrush(Color.FromRgb(40, 40, 40));
            app.Resources["InputBorder"] = new SolidColorBrush(Color.FromRgb(70, 70, 70));
        }
        else
        {
            app.Resources["WindowBackground"] = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            app.Resources["CardBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            app.Resources["HeaderBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            app.Resources["TextPrimary"] = new SolidColorBrush(Color.FromRgb(33, 33, 33));
            app.Resources["TextSecondary"] = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            app.Resources["BorderColor"] = new SolidColorBrush(Color.FromRgb(224, 224, 224));
            app.Resources["HoverBackground"] = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            app.Resources["DataGridBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            app.Resources["DataGridHeaderBackground"] = new SolidColorBrush(Color.FromRgb(55, 71, 79));
            app.Resources["DataGridHeaderForeground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            app.Resources["DataGridAlternateRow"] = new SolidColorBrush(Color.FromRgb(250, 250, 250));
            app.Resources["DataGridRowBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            app.Resources["DataGridRowHover"] = new SolidColorBrush(Color.FromRgb(245, 245, 245));
            app.Resources["DataGridRowSelected"] = new SolidColorBrush(Color.FromRgb(187, 222, 251));
            app.Resources["DataGridCellSelected"] = new SolidColorBrush(Color.FromRgb(227, 242, 253));
            app.Resources["DataGridGridLines"] = new SolidColorBrush(Color.FromRgb(236, 239, 241));
            app.Resources["InputBackground"] = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            app.Resources["InputBorder"] = new SolidColorBrush(Color.FromRgb(207, 216, 220));
        }
    }

    public static void Initialize()
    {
        ApplyTheme();
    }
}
