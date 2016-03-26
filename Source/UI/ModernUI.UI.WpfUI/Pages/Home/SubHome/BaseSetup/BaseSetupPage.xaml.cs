using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class BaseSetupPage : UserControl
    {
        private BaseSetupViewModel viewModel
        {
            get
            {
                return this.DataContext as BaseSetupViewModel;
            }
        }

        public BaseSetupPage()
        {
            InitializeComponent();
            this.DataContext = ViewModelService.GetViewModel<BaseSetupViewModel>();
            this.IsVisibleChanged += BaseSetupPage_IsVisibleChanged;
        }

        void BaseSetupPage_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                ViewModelService.GetViewModel<HomePageViewModel>().Header = "Setup your workspace";
            }
        }
    }
}
