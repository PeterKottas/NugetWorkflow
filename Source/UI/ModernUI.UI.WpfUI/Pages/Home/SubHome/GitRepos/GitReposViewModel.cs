using FirstFloor.ModernUI.Presentation;
using Microsoft.Win32;
using NugetWorkflow.UI.WpfUI.Common.Interfaces;
using NugetWorkflow.UI.WpfUI.Common.Extensions;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.DTOs;
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

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos
{
    public class GitReposViewModel : NotifyPropertyChanged, IView
    {
        private JavaScriptSerializer serializer;
        private ObservableCollection<GitRepoDTO> gitRepos;
        private bool includePassword = false;
        private string overidenUsername = string.Empty;
        public RelayCommand ExportJsonCommand { get; set; }
        public RelayCommand ExportJsonClipboardCommand { get; set; }
        public RelayCommand ImportJsonCommand { get; set; }
        public RelayCommand ImportJsonClipboardCommand { get; set; }
        public RelayCommand AddRowCommand { get; set; }
        public RelayCommand RemoveRowCommand { get; set; }
        
        public string OveridenUsername
        {
            get
            {
                return overidenUsername;
            }
            set
            {
                if (value != null)
                {
                    overidenUsername = value;
                    OnPropertyChanged("OveridenUsername");
                }
            }
        }

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
                return !IncludePassword ? "Export Json" : "Export Json - Encryption isn't perfect for now!";
            }
        }

        public ObservableCollection<GitRepoDTO> GitRepos
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
                var Json = GetJson();
                Clipboard.SetText(Json);
            }
        }

        private string GetJson()
        {
            var list = new List<dynamic>();
            foreach (var item in gitRepos)
            {
                var password = item.Password.ToUnsecuredString().Protect();
                if (IncludePassword)
                {
                    list.Add(new { item.Url, item.Username, password, item.UseOverrideCredentials });
                }
                else
                {
                    list.Add(new { item.Url, item.Username, item.UseOverrideCredentials });
                }
            }
            var Json = serializer.Serialize(list);
            return Json;
        }

        private void ExportJsonExecute(object parameter)
        {
            if (gitRepos != null && gitRepos.Count > 0)
            {
                var Json = GetJson();
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

        public BaseSetupViewModel homeViewModel { get; set; }

        public GitReposViewModel()
        {
            this.homeViewModel = ViewModelService.GetViewModel<BaseSetupViewModel>();
            gitRepos = new ObservableCollection<GitRepoDTO>();
            gitRepos.Add(new GitRepoDTO() { Url = "https://github.com/cloudera/repository-example.git", });
            ExportJsonCommand = new RelayCommand(ExportJsonExecute, ExportJsonCanExecute);
            ExportJsonClipboardCommand = new RelayCommand(ExportJsonClipboardExecute, ExportJsonCanExecute);
            RemoveRowCommand = new RelayCommand(RemoveRowExecute);
            AddRowCommand = new RelayCommand(AddRowExectue);
            serializer = new JavaScriptSerializer();
        }

        private void AddRowExectue(object obj)
        {
            GitRepos.Add(new GitRepoDTO());
        }

        private void RemoveRowExecute(object obj)
        {
            var ID = obj.ToString();
            var row = GitRepos.Where(dto => dto.Hash == ID).FirstOrDefault();
            GitRepos.Remove(row);
        }

    }
}
