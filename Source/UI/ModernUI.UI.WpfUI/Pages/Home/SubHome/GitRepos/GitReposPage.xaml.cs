﻿using NugetWorkflow.UI.WpfUI.Utils;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos
{
    /// <summary>
    /// Interaction logic for GitRepos.xaml
    /// </summary>
    public partial class GitReposPage : UserControl
    {
        private GitReposViewModel viewModel
        {
            get
            {
                return this.DataContext as GitReposViewModel;
            }
        }

        public GitReposPage()
        {
            InitializeComponent();
            //container = ((App)Application.Current).Container;
            this.DataContext = ViewModelService.GetViewModel<GitReposViewModel>();
        }
    }
}
