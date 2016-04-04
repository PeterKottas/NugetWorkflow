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
            InitializeComponent();
            //container = ((App)Application.Current).Container;
            this.IsVisibleChanged += ClonePage_IsVisibleChanged;
            AssignViewModel();
        }

        void ClonePage_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ViewModelService.GetViewModel<HomePageViewModel>().Header = "Now lets clone some serious code!";
            }
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<CloneViewModel>();
        }
    }
}
