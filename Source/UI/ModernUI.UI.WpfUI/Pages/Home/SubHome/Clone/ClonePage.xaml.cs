﻿using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Utils;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Clone
{
    /// <summary>
    /// Interaction logic for GitRepos.xaml
    /// </summary>
    public partial class ClonePage : UserControl
    {
        public GitReposViewModel GitReposVM
        {
            get
            {
                return ViewModelService.GetViewModel<GitReposViewModel>();
            }
        }

        public BaseSetupViewModel BaseSetupVM
        {
            get
            {
                return ViewModelService.GetViewModel<BaseSetupViewModel>();
            }
        }

        public ClonePage()
        {
            InitializeComponent();
            //container = ((App)Application.Current).Container;
            this.DataContext = ViewModelService.GetViewModel<CloneViewModel>();
            this.IsVisibleChanged += ClonePage_IsVisibleChanged;
        }

        void ClonePage_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ViewModelService.GetViewModel<HomePageViewModel>().Header = "Now lets clone some serious code!";
            }
        }
    }
}
