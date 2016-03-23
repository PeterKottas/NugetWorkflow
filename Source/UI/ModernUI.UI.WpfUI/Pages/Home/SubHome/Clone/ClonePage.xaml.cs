using NugetWorkflow.UI.WpfUI.Utils;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Clone
{
    /// <summary>
    /// Interaction logic for GitRepos.xaml
    /// </summary>
    public partial class ClonePage : UserControl
    {
        private CloneViewModel viewModel
        {
            get
            {
                return this.DataContext as CloneViewModel;
            }
        }

        public ClonePage()
        {
            InitializeComponent();
            //container = ((App)Application.Current).Container;
            this.DataContext = ViewModelService.GetViewModel<CloneViewModel>();
        }
    }
}
