using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LineManagementSystem.Models;
using LineManagementSystem.Services;
using LineManagementSystem.ViewModels;

namespace LineManagementSystem.Views;

public partial class GroupDetailsWindow : Window
{
    private readonly GroupDetailsViewModel _viewModel;
    private readonly ProviderGroupsViewModel _parentViewModel;
    private readonly AlertService _alertService;

    public GroupDetailsWindow(LineGroup group, ProviderGroupsViewModel parentViewModel, AlertService alertService)
    {
        InitializeComponent();

        var context = new DatabaseContext();
        // نتأكد إن الـ context نظيف من أي tracking قديم
        context.ChangeTracker.Clear();

        var groupService = new GroupService(context);
        _viewModel = new GroupDetailsViewModel(groupService, group);
        _parentViewModel = parentViewModel;
        _alertService = alertService;

        _viewModel.LineAdded += () =>
        {
            _parentViewModel.LoadGroups();
            txtPhoneNumber.Focus();
        };

        DataContext = _viewModel;
    }

    protected override void OnContentRendered(EventArgs e)
    {
        base.OnContentRendered(e);
        if (_viewModel.IsAddingLine)
        {
            txtPhoneNumber.Focus();
        }
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var textBox = sender as TextBox;
            if (textBox == txtName)
            {
                txtNationalId.Focus();
            }
            else if (textBox == txtNationalId)
            {
                txtPhoneNumber.Focus();
            }
            else if (textBox == txtPhoneNumber)
            {
                txtInternalId.Focus();
            }
            else if (textBox == txtInternalId)
            {
                if (_viewModel.SaveLineCommand.CanExecute(null))
                {
                    _viewModel.SaveLineCommand.Execute(null);
                }
            }
            else if (textBox == txtCashWalletNumber)
            {
                txtName.Focus();
            }
        }
    }

    private void BtnConfirm1_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.NewLine != null)
        {
            _viewModel.NewLine.ConfirmationLevel = 1;
        }
    }

    private void BtnConfirm2_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.NewLine != null)
        {
            _viewModel.NewLine.ConfirmationLevel = 2;
        }
    }

    private void BtnConfirm3_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.NewLine != null)
        {
            _viewModel.NewLine.ConfirmationLevel = 3;
        }
    }

    private void BtnConfirmClear_Click(object sender, RoutedEventArgs e)
    {
        if (_viewModel.NewLine != null)
        {
            _viewModel.NewLine.ConfirmationLevel = 0;
        }
    }

    private void Window_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && !_viewModel.IsAddingLine)
        {
            if (_viewModel.AddLineCommand.CanExecute(null))
            {
                _viewModel.AddLineCommand.Execute(null);

                // التركيز على خانة رقم الخط بعد فتح النموذج
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    txtPhoneNumber.Focus();
                    txtPhoneNumber.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Placeholder for back button functionality if needed
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _alertService.CheckForAlerts();
    }

    private void ExportExcel_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            $"قريباً: تصدير خطوط مجموعة '{_viewModel.Group.Name}' إلى Excel\n\nسيتم تصدير جميع بيانات الخطوط بتنسيق احترافي",
            "ميزة قادمة",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void ExportPDF_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(
            $"قريباً: تصدير خطوط مجموعة '{_viewModel.Group.Name}' إلى PDF\n\nسيتم إنشاء تقرير PDF مفصل",
            "ميزة قادمة",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedLine != null)
        {
            if (_viewModel.EditLineCommand.CanExecute(null))
            {
                _viewModel.EditLineCommand.Execute(null);
            }
        }
    }
}