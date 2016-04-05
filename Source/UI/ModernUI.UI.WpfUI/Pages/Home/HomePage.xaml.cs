using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl, IPageUserControl
    {
        private string currentSource = string.Empty;
        
        public HomePage()
        {
            AssignViewModel();
            InitializeComponent();
            AddUserControl(this);
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<HomePageViewModel>();
            ViewModelService.GetViewModel<HomePageViewModel>().UpdateHeader(currentSource);
        }

        public void AddUserControl(IPageUserControl userControl)
        {
            PageUserControlService.AddUserControl(this);
        }

        private void ModernTab_SelectedSourceChanged(object sender, FirstFloor.ModernUI.Windows.Controls.SourceEventArgs e)
        {
            currentSource = e.Source.ToString();
            ViewModelService.GetViewModel<HomePageViewModel>().UpdateHeader(currentSource);
        }
    }
}
