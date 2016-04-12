using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Utils;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Update
{
    /// <summary>
    /// Interaction logic for GitRepos.xaml
    /// </summary>
    public partial class UpdatePage : UserControl, IPageUserControl
    {
        public UpdatePage()
        {
            AssignViewModel();
            InitializeComponent();
            AddUserControl();
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<UpdateViewModel>();
        }

        public void AddUserControl()
        {
            PageUserControlService.AddUserControl(this);
        }
    }
}
