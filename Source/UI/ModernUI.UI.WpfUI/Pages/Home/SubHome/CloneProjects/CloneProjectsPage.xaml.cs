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
using NugetWorkflow.UI.WpfUI.Pages.CLoneProjects;
using NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Models;
using NugetWorkflow.UI.WpfUI.Common.Base;
using NugetWorkflow.UI.WpfUI.Utils;

namespace NugetWorkflow.UI.WpfUI.Pages.Settings.CloneProjects
{
    /// <summary>
    /// Interaction logic for CloneProjects.xaml
    /// </summary>
    public partial class CloneProjectsPage : UserControl
    {
        private CloneProjectsViewModel viewModel
        {
            get
            {
                return this.DataContext as CloneProjectsViewModel;
            }
        }


        public CloneProjectsPage()
        {
            InitializeComponent();
            //container = ((App)Application.Current).Container;
            this.DataContext = ViewModelService.GetViewModel<CloneProjectsViewModel>();
        }

        private void RemoveRow(object sender, RoutedEventArgs e)
        {
            var ID = (string)((Button)sender).CommandParameter;
            var row = viewModel.GitRepos.Where(dto => dto.Hash == ID).FirstOrDefault();
            viewModel.GitRepos.Remove(row);
        }

        private void AddRow(object sender, RoutedEventArgs e)
        {
            viewModel.GitRepos.Add(new GitRepoViewModelDTO());
        }
    }
}
