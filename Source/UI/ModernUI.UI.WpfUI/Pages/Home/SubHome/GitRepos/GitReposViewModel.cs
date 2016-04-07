using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;
using NugetWorkflow.Common.Base.Converters.JavaScriptConverters;
using NugetWorkflow.Common.Base.Extensions;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Attributes;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Models;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Input;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos
{
    [SaveSceneAttribute]
    public class GitReposViewModel : BaseViewModel, IViewModel
    {
        //Private properties
        private JavaScriptSerializer serializer;

        private JavaScriptSerializer deSerializer;
        //\Private properties

        //Data hiding
        private ObservableCollection<GitRepoModel> gitRepos;

        private bool includePassword = false;

        //private string overidenUsername = "admin";
        private string overidenUsername = "peterkottas";

        //private SecureString overridenPassword = "Betfred1".ToSecuredString();
        private SecureString overridenPassword = "test".ToSecuredString();

        private List<string> nuGetPackagesIDsUnion;

        private List<string> repoBranchesUnion;
        //\Data hiding

        //Properties names
        public static readonly string OverridenPasswordPropName = ReflectionUtility.GetPropertyName((GitReposViewModel s) => s.OverridenPassword);

        public static readonly string OveridenUsernamePropName = ReflectionUtility.GetPropertyName((GitReposViewModel s) => s.OveridenUsername);

        public static readonly string IncludePasswordPropName = ReflectionUtility.GetPropertyName((GitReposViewModel s) => s.IncludePassword);

        public static readonly string ExportHeaderPropName = ReflectionUtility.GetPropertyName((GitReposViewModel s) => s.ExportHeader);

        public static readonly string GitReposPropName = ReflectionUtility.GetPropertyName((GitReposViewModel s) => s.GitRepos);

        public static readonly string NuGetPackagesIDsUnionPropName = ReflectionUtility.GetPropertyName((GitReposViewModel s) => s.NuGetPackagesIDsUnion);

        public static readonly string RepoBranchesUnionPropName = ReflectionUtility.GetPropertyName((GitReposViewModel s) => s.RepoBranchesUnion);
        //\Properties names

        //Commands
        public RelayCommand ExportJsonCommand { get; set; }

        public RelayCommand ExportJsonClipboardCommand { get; set; }

        public RelayCommand ImportJsonCommand { get; set; }

        public RelayCommand ImportJsonClipboardCommand { get; set; }

        public RelayCommand AddRowCommand { get; set; }

        public RelayCommand RemoveRowCommand { get; set; }
        //\Commands

        //Bindable properties
        public string Header
        {
            get
            {
                return "Setup your git server connections";
            }
        }

        public List<string> RepoBranchesUnion
        {
            get
            {
                return repoBranchesUnion;
            }
            set
            {
                repoBranchesUnion = value;
                OnPropertyChanged(RepoBranchesUnionPropName);
            }
        }

        public List<string> NuGetPackagesIDsUnion
        {
            get
            {
                return nuGetPackagesIDsUnion;
            }
            set
            {
                nuGetPackagesIDsUnion = value;
                OnPropertyChanged(NuGetPackagesIDsUnionPropName);
            }
        }

        [SaveSceneAttribute]
        public SecureString OverridenPassword
        {
            get
            {
                return overridenPassword;
            }
            set
            {
                var orig = overridenPassword;
                overridenPassword = value;
                OnUndoRedoPropertyChanged(OverridenPasswordPropName, () => overridenPassword = orig, () => overridenPassword = value);
            }
        }

        [SaveSceneAttribute]
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
                    var orig = overidenUsername;
                    overidenUsername = value;
                    OnUndoRedoPropertyChanged(OveridenUsernamePropName, () => overidenUsername = orig, () => overidenUsername = value);
                }
            }
        }

        [SaveSceneAttribute]
        public bool IncludePassword
        {
            get
            {
                return includePassword;
            }
            set
            {
                var orig = includePassword;
                includePassword = value;
                OnUndoRedoPropertyChanged(IncludePasswordPropName, () => includePassword = orig, () => includePassword = value);

                OnPropertyChanged(ExportHeaderPropName);
            }
        }

        public string ExportHeader
        {
            get
            {
                return !IncludePassword ? "Export Json" : "Export Json - Encryption isn't perfect for now!";
            }
        }

        [SaveSceneAttribute]
        public ObservableCollection<GitRepoModel> GitRepos
        {
            get
            {
                return this.gitRepos;
            }
            set
            {
                var orig = gitRepos;
                gitRepos = value;
                OnUndoRedoPropertyChanged(GitReposPropName, () => gitRepos = orig, () => gitRepos = value);
            }
        }

        public BaseSetupViewModel homeViewModel { get; set; }
        //\Bindable properties

        //Implementation
        public GitReposViewModel()
        {
            gitRepos = new ObservableCollection<GitRepoModel>();
            gitRepos.Add(new GitRepoModel() { Url = "https://github.com/PeterKottas/NugetWorkflow.git" });

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

        public void Initialize()
        {
            this.homeViewModel = ViewModelService.GetViewModel<BaseSetupViewModel>();
        }

        private void LoadModelFromJson(string Json)
        {
            if (!string.IsNullOrEmpty(Json.Replace(" ", "")))
            {
                try
                {
                    var jsonObject = GetObject(Json);
                    var GitReposLocal = new ObservableCollection<GitRepoModel>();
                    foreach (dynamic repo in jsonObject["GitReposJson"])
                    {
                        GitReposLocal.Add(new GitRepoModel()
                        {
                            Url = repo.Url,
                            UseOverrideCredentials = repo.UseOverrideCredentials,
                            Username = repo.Username,
                            Password = string.IsNullOrEmpty(repo.Password) ? string.Empty.ToSecuredString() : ((string)repo.Password).Unprotect().ToSecuredString()
                        });
                    }
                    GitRepos = GitReposLocal;

                    /*var username = jsonObject["DefaultCredJson"]["Username"];
                    var password = ((string)jsonObject["DefaultCredJson"]["Password"]).Unprotect().ToSecuredString();
                    OveridenUsername = username;
                    OverridenPassword = password;*/
                }
                catch (Exception)
                {
                    ModernDialog.ShowMessage("Seems the text in you clipboard is not in right format. Try again please", "Json format is incorrect", MessageBoxButton.OK);
                }
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
                    /*DefaultCredJson =
                    new
                    {
                        Username = OveridenUsername,
                        Password = IncludePassword ? overridenPassword.ToUnsecuredString().Protect() : string.Empty
                    }*/
                });
            return Json;
        }

        private dynamic GetObject(string Json)
        {
            dynamic Object = deSerializer.DeserializeObject(Json);
            return Object;
        }

        public void RefreshBindings()
        {
            OnPropertyChanged(string.Empty);
        }

        public void UpdateStatuses(string BasePath)
        {
            foreach (var repo in GitRepos)
            {
                repo.UpdateSetupStatus(BasePath);
            }
        }

        public void UpdateStatuses()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            UpdateStatuses(basePath);
        }

        public void UpdatePackages(string BasePath)
        {
            foreach (var repo in GitRepos)
            {
                repo.UpdatePackagesIDs(BasePath);
            }
            InvalidateNuGetPackagesIDsUnion();
        }

        public void InvalidateNuGetPackagesIDsUnion()
        {
            NuGetPackagesIDsUnion = GitRepos.SelectMany(s => s.NuGetVersions).Distinct().ToList();
        }

        public void UpdatePackages()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            UpdatePackages(basePath);
        }

        public void InvalidateRepobranchesUnion()
        {
            RepoBranchesUnion = GitRepos.SelectMany(s => s.RepoBranches).Distinct().ToList();
        }

        public void UpdateRepoBranches(string BasePath)
        {
            foreach (var repo in GitRepos)
            {
                repo.UpdateRepoBranches(BasePath);
            }
            InvalidateRepobranchesUnion();
        }

        public void UpdateRepoBranches()
        {
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            UpdateRepoBranches(basePath);
        }
        //Implementation

        //Commands logic
        private void ExportJsonClipboardExecute(object parameter)
        {
            if (gitRepos != null && gitRepos.Count > 0)
            {
                var Json = GetJson();
                Clipboard.SetText(Json);
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

        private bool ImportJsonClipboardCanExecute(object arg)
        {
            return !string.IsNullOrEmpty(Clipboard.GetText().Replace(" ", string.Empty));
        }

        private void ImportJsonClipboardExecute(object obj)
        {
            var Json = Clipboard.GetText();
            LoadModelFromJson(Json);
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

        private void AddRowExectue(object obj)
        {
            var newRepo = new GitRepoModel();
            GitRepos.Add(newRepo);
            OnUndoRedoPropertyChanged(GitReposPropName, ()=> gitRepos.Remove(gitRepos.Where(a=>a.Hash==newRepo.Hash).FirstOrDefault()), ()=>gitRepos.Add(newRepo));
        }

        private void RemoveRowExecute(object obj)
        {
            var ID = obj.ToString();
            var row = GitRepos.Where(dto => dto.Hash == ID).FirstOrDefault();
            GitRepos.Remove(row);
            OnUndoRedoPropertyChanged(GitReposPropName, () => gitRepos.Add(row), () => gitRepos.Remove(row));
        }
        //\Commands logic
    }
}
