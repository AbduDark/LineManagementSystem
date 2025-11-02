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

        var color = group.Provider.GetColorHex();
        
        // تطبيق لون خلفية خفيف مناسب لكل مزود
        var backgroundColor = group.Provider switch
        {
            TelecomProvider.Vodafone => "#FFF5F5",    // خلفية حمراء فاتحة جداً
            TelecomProvider.Etisalat => "#F1F8E9",    // خلفية خضراء فاتحة جداً
            TelecomProvider.We => "#F3E5F5",          // خلفية بنفسجية فاتحة جداً
            TelecomProvider.Orange => "#FFF3E0",      // خلفية برتقالية فاتحة جداً
            _ => "#F5F5F5"
        };
        
        Background = new System.Windows.Media.SolidColorBrush(
            (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(backgroundColor)
        );
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

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _alertService.CheckForAlerts();
    }
}
