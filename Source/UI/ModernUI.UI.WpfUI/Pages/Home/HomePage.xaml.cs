using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
            try
            {
                this.DataContext = ViewModelService.GetViewModel<HomePageViewModel>();
            }
            catch (Exception)
            {
                this.DataContext = new HomePageViewModel();
            }
        }
    }
}
