﻿using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Utils;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos
{
    /// <summary>
    /// Interaction logic for GitRepos.xaml
    /// </summary>
    public partial class GitReposPage : UserControl, IPageUserControl
    {
        public GitReposPage()
        {
            AssignViewModel();
            InitializeComponent();
            AddUserControl();
        }

        public void AssignViewModel()
        {
            this.DataContext = ViewModelService.GetViewModel<GitReposViewModel>();
        }

        public void AddUserControl()
        {
            PageUserControlService.AddUserControl(this);
        }

        private void DataGrid_UnloadingRow(object sender, DataGridRowEventArgs e)
        {
            PageUserControlService.GetUserControl<HomePage>().Focus();
        }
    }
}
