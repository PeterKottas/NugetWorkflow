using FirstFloor.ModernUI.Windows.Controls;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Pages.Home;
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

namespace NugetWorkflow.UI.WpfUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow, IPageUserControl
    {
        public MainWindow()
        {
            InitializeComponent();
            AssignViewModel();
            AddUserControl();
            this.Closing += MainWindow_Closing;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!SceneSaver.HandleDirtySceneOverWrite())
            {
                e.Cancel = true;
            }
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<MainWindowViewModel>();
        }

        public void AddUserControl()
        {
            PageUserControlService.AddUserControl(this);
        }
    }
}
