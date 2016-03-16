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

namespace NugetWorkflow.UI.WpfUI.Pages.Settings
{
    /// <summary>
    /// Interaction logic for CloneProjects.xaml
    /// </summary>
    public partial class CloneProjects : UserControl
    {
        private CloneProjectsViewModel viewModel
        {
            get
            {
                return this.DataContext as CloneProjectsViewModel;
            }
        }
        public CloneProjects()
        {
            InitializeComponent();
            this.DataContext = new CloneProjectsViewModel();
        }

        private void ChooseBaseFolder(object sender, RoutedEventArgs e)
        {

            var dlg = new CommonOpenFileDialog();
            dlg.Title = "My Title";
            dlg.IsFolderPicker = true;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var folder = dlg.FileName;
                // Do something with selected folder string
            }
        }

        private void RemoveRow(object sender, RoutedEventArgs e)
        {
            var ID = (string)((Button)sender).CommandParameter;
            var row = viewModel.TestList.Where(dto=>dto.Hash==ID).FirstOrDefault();
            viewModel.TestList.Remove(row);
        }

        private void AddRow(object sender, RoutedEventArgs e)
        {
            viewModel.TestList.Add(new GitRepoViewModelDTO());
        }
    }
}
