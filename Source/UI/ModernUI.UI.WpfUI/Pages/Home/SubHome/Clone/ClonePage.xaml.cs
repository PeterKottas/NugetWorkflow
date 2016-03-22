using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NugetWorkflow.UI.WpfUI.Pages;
using NugetWorkflow.UI.WpfUI.Extensions;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Data;
using NugetWorkflow.UI.WpfUI.Common.Base;
using NugetWorkflow.UI.WpfUI.Utils;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Models;

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
