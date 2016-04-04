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
            InitializeComponent();
            //container = ((App)Application.Current).Container;
            this.IsVisibleChanged += GitReposPage_IsVisibleChanged;
            AssignViewModel();
        }

        void GitReposPage_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ViewModelService.GetViewModel<HomePageViewModel>().Header = "Setup your git server connections";
            }
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<GitReposViewModel>();
        }
    }
}
