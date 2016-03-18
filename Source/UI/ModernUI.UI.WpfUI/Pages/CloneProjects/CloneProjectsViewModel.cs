using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI.Pages.CLoneProjects
{
    public class CloneProjectsViewModel : NotifyPropertyChanged
    {
        static Func<string> BasePathFunc = () => basePath;
        private ObservableCollection<GitRepoViewModelDTO> gitRepos;
        private static string basePath;

        public string BasePath
        {
            get 
            { 
                return basePath;
            }
            set
            {
                basePath=value;
                OnPropertyChanged("BasePath");
            }
        }

        public ObservableCollection<GitRepoViewModelDTO> GitRepos
        {
            get
            {
                return this.gitRepos;
            }
            set
            {
                gitRepos = value;
                OnPropertyChanged("GitRepos");
            }
        }

        public CloneProjectsViewModel()
        {
            gitRepos = new ObservableCollection<GitRepoViewModelDTO>();
            gitRepos.Add(new GitRepoViewModelDTO() { Url = "https://github.com/cloudera/repository-example.git", });
        }
    }
}
