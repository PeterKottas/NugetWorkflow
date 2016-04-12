using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Utils;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Clone
{
    /// <summary>
    /// Interaction logic for GitRepos.xaml
    /// </summary>
    public partial class ClonePage : UserControl, IPageUserControl
    {
        public ClonePage()
        {
            AssignViewModel();
            InitializeComponent();
            AddUserControl();
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<CloneViewModel>();
        }
        
        public void AddUserControl()
        {
            PageUserControlService.AddUserControl(this);
        }
    }
}
