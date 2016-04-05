using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using NugetWorkflow.Common.Base.Interfaces;
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
    public partial class BaseSetupPage : UserControl, IPageUserControl
    {
        public BaseSetupPage()
        {
            AssignViewModel();
            InitializeComponent();
            AddUserControl(this);
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<BaseSetupViewModel>();
        }

        public void AddUserControl(IPageUserControl userControl)
        {
            PageUserControlService.AddUserControl(this);
        }
    }
}
