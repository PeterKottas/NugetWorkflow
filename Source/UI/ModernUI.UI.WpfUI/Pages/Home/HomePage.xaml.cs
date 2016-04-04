using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl, IPageUserControl
    {
        public HomePage()
        {
            InitializeComponent();
            AssignViewModel();
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<HomePageViewModel>();
        }
    }
}
