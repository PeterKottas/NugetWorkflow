using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : UserControl, IPageUserControl
    {
        public HomePage()
        {
            AssignViewModel();
            InitializeComponent();
            AddUserControl();
            Focusable = true;
            Loaded += (s, e) => Keyboard.Focus(this);
            LostKeyboardFocus += (s, e) => Trace.WriteLine("Lost focus");
            GotKeyboardFocus += (s, e) => Trace.WriteLine("Got focus");
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<HomePageViewModel>();
        }

        public void AddUserControl()
        {
            PageUserControlService.AddUserControl(this);
        }

        private void ModernTab_SelectedSourceChanged(object sender, FirstFloor.ModernUI.Windows.Controls.SourceEventArgs e)
        {
            this.Focus();
            //Keyboard.Focus(this);
        }

        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Trace.WriteLine(e.Key);
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if(e.Key==Key.Z)
                {
                    ViewModelService.GetViewModel<HomePageViewModel>().UndoCommand.Execute(null);
                }
                else if (e.Key == Key.Y)
                {
                    ViewModelService.GetViewModel<HomePageViewModel>().RedoCommand.Execute(null);
                }
                else if (e.Key == Key.O)
                {
                    ViewModelService.GetViewModel<HomePageViewModel>().OpenFileCommand.Execute(null);
                }
                else if (e.Key == Key.N)
                {
                    ViewModelService.GetViewModel<HomePageViewModel>().NewFileCommand.Execute(null);
                }
                else if (e.Key == Key.S)
                {
                    ViewModelService.GetViewModel<HomePageViewModel>().SaveFileCommand.Execute(null);
                }
            }
        }
    }
}
