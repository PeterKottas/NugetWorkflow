using FirstFloor.ModernUI.Presentation;
using Microsoft.Win32;
using NugetWorkflow.UI.WpfUI.Common.Interfaces;
using NugetWorkflow.UI.WpfUI.Extensions;
using NugetWorkflow.UI.WpfUI.Pages.CloneProjects.Models;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI.Pages.CLoneProjects
{
    public class CloneProjectsViewModel : NotifyPropertyChanged, IView
    {
        private JavaScriptSerializer serializer;
        private ObservableCollection<GitRepoViewModelDTO> gitRepos;
        private bool includePassword = false;
        public RelayCommand ExportJsonCommand { get; set; }
        public RelayCommand ExportJsonClipboardCommand { get; set; }
        public RelayCommand ImportJsonCommand { get; set; }
        public RelayCommand ImportJsonClipboardCommand { get; set; }



        public bool IncludePassword
        {
            get
            {
                return includePassword;
            }
            set
            {
                includePassword = value;
                OnPropertyChanged("IncludePassword");
                OnPropertyChanged("ExportHeader");
            }
        }

        public string ExportHeader
        {
            get
            {
                return !IncludePassword ? "Export Json" : "Export Json - Passwords are unencrypted!";
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

        private void ExportJsonClipboardExecute(object parameter)
        {
            if (gitRepos != null && gitRepos.Count > 0)
            {
                var list = new List<dynamic>();
                foreach (var item in gitRepos)
                {
                    var password = item.Password.ToUnsecuredString();
                    list.Add(new { item.Url, item.Username, password });
                }
                var Json = serializer.Serialize(list);
                Clipboard.SetText(Json);
            }
        }

        private void ExportJsonExecute(object parameter)
        {
            if (gitRepos != null && gitRepos.Count > 0)
            {
                var list = new List<dynamic>();
                foreach (var item in gitRepos)
	            {
                    var password = item.Password.ToUnsecuredString();
		            list.Add(new{ item.Url, item.Username, password});
	            }
                var Json = serializer.Serialize(list);
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "GitReposJson"; // Default file name
                saveFileDialog.DefaultExt = ".txt"; // Default file extension
                saveFileDialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
                var result = saveFileDialog.ShowDialog();
                if (result.HasValue && (bool)result.Value)
                {
                    File.WriteAllText(saveFileDialog.FileName, Json);
                }
            }
        }

        private bool ExportJsonCanExecute(object parameter)
        {
            if (gitRepos != null && gitRepos.Count > 0)
            {
                return true;
            }
            return false;
        }

        public HomePageViewModel homeViewModel { get; set; }

        public CloneProjectsViewModel()
        {
            this.homeViewModel = ViewModelService.GetViewModel<HomePageViewModel>();
            gitRepos = new ObservableCollection<GitRepoViewModelDTO>();
            gitRepos.Add(new GitRepoViewModelDTO() { Url = "https://github.com/cloudera/repository-example.git", });
            ExportJsonCommand = new RelayCommand(ExportJsonExecute, ExportJsonCanExecute);
            ExportJsonClipboardCommand = new RelayCommand(ExportJsonClipboardExecute, ExportJsonCanExecute);
            serializer = new JavaScriptSerializer();
        }

    }
}
