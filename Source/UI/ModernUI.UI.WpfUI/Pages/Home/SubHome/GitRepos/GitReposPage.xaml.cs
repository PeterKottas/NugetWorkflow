using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Utils;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos
{
    /// <summary>
    /// Interaction logic for GitRepos.xaml
    /// </summary>
    public partial class GitReposPage : UserControl, IPageUserControl
    {
        public GitReposPage()
        {
            AssignViewModel();
            InitializeComponent();
            AddUserControl(this);
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<GitReposViewModel>();
        }

        public void AddUserControl(IPageUserControl userControl)
        {
            PageUserControlService.AddUserControl(this);
        }
    }
}
