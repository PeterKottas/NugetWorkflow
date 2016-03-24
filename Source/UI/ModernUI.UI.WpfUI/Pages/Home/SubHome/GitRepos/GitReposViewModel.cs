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
using FirstFloor.ModernUI.Windows.Controls;
using NugetWorkflow.UI.WpfUI.Common.Converters.JavaScriptConverters;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos
{
    public class GitReposViewModel : NotifyPropertyChanged, IView
    {
        private JavaScriptSerializer serializer;
        private JavaScriptSerializer deSerializer;
        private ObservableCollection<GitRepoDTO> gitRepos;
        private bool includePassword = false;
        private string overidenUsername = string.Empty;
        private SecureString overridenPassword;
        public RelayCommand ExportJsonCommand { get; set; }
        public RelayCommand ExportJsonClipboardCommand { get; set; }
        public RelayCommand ImportJsonCommand { get; set; }
        public RelayCommand ImportJsonClipboardCommand { get; set; }
        public RelayCommand AddRowCommand { get; set; }
        public RelayCommand RemoveRowCommand { get; set; }

        public SecureString OverridenPassword
        {
            get
            {
                return overridenPassword;
            }
            set
            {
                overridenPassword = value;
                OnPropertyChanged("OverridenPassword");
            }
        }

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
                if (IncludePassword)
                {
                    var password = item.Password.ToUnsecuredString().Protect();
                    list.Add(new { Url = item.Url, Username = item.Username, Password = password, UseOverrideCredentials = item.UseOverrideCredentials });
                }
                else
                {
                    list.Add(new { Url = item.Url, Username = item.Username, Password = string.Empty, UseOverrideCredentials = item.UseOverrideCredentials });
                }
            }
            var Json = serializer.Serialize(
                new
                {
                    GitReposJson = list,
                    DefaultCredJson =
                    new
                    {
                        Username = OveridenUsername,
                        Password = IncludePassword ? overridenPassword.ToUnsecuredString().Protect() : string.Empty
                    }
                });
            return Json;
        }

        private dynamic GetObject(string Json)
        {
            dynamic Object = deSerializer.DeserializeObject(Json);
            return Object;
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
            gitRepos.Add(new GitRepoDTO());
            ImportJsonClipboardCommand = new RelayCommand(ImportJsonClipboardExecute, ImportJsonClipboardCanExecute);
            ImportJsonCommand = new RelayCommand(ImportJsonExecute);
            ExportJsonCommand = new RelayCommand(ExportJsonExecute, ExportJsonCanExecute);
            ExportJsonClipboardCommand = new RelayCommand(ExportJsonClipboardExecute, ExportJsonCanExecute);
            RemoveRowCommand = new RelayCommand(RemoveRowExecute);
            AddRowCommand = new RelayCommand(AddRowExectue);
            serializer = new JavaScriptSerializer();
            deSerializer = new JavaScriptSerializer();
            deSerializer.RegisterConverters(new[] { new DynamicJsonConverter() });
        }

        private void ImportJsonExecute(object obj)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.FileName = "GitReposJson"; // Default file name
            openFileDialog.DefaultExt = ".txt"; // Default file extension
            openFileDialog.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
            var result = openFileDialog.ShowDialog();
            if (result.HasValue && (bool)result.Value)
            {
                var Json = File.ReadAllText(openFileDialog.FileName);
                LoadModelFromJson(Json);
            }
        }

        private void LoadModelFromJson(string Json)
        {
            if (!string.IsNullOrEmpty(Json.Replace(" ", "")))
            {
                try
                {
                    var jsonObject = GetObject(Json);
                    var GitReposLocal = new ObservableCollection<GitRepoDTO>();
                    foreach (dynamic repo in jsonObject["GitReposJson"])
                    {
                        GitReposLocal.Add(new GitRepoDTO()
                        {
                            Url = repo.Url,
                            UseOverrideCredentials = repo.UseOverrideCredentials,
                            Username = repo.Username,
                            Password = string.IsNullOrEmpty(repo.Password) ? string.Empty.ToSecuredString() : ((string)repo.Password).Unprotect().ToSecuredString()
                        });
                    }
                    var username = jsonObject["DefaultCredJson"]["Username"];
                    var password = ((string)jsonObject["DefaultCredJson"]["Password"]).Unprotect().ToSecuredString();
                    GitRepos = GitReposLocal;
                    OveridenUsername = username;
                    OverridenPassword = password;
                }
                catch (Exception)
                {
                    ModernDialog.ShowMessage("Seems the text in you clipboard is not in right format. Try again please", "Json format is incorrect", MessageBoxButton.OK);
                }
            }
        }

        private bool ImportJsonClipboardCanExecute(object arg)
        {
            return !string.IsNullOrEmpty(Clipboard.GetText().Replace(" ", string.Empty));
        }

        private void ImportJsonClipboardExecute(object obj)
        {
            var Json = Clipboard.GetText();
            LoadModelFromJson(Json);
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
