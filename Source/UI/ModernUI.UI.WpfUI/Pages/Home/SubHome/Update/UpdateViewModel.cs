using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.Attributes;
using NugetWorkflow.Common.Base.DTOs.GitRepos;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.Common.GitAdapter.DTOs.Requests;
using NugetWorkflow.Common.GitAdapter.Interfaces;
using NugetWorkflow.Plugins.GitAdapter;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Models;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Shared.Enums;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Update
{
    [SaveSceneAttribute]
    public class UpdateViewModel : NotifyPropertyChanged, IViewModel
    {
        //Dependencies interfaces
        private IGitAdapter gitAdapter;
        //\Dependencies interfaces

        //Private properties
        private bool calcRunning = false;
        private TimerUpdater uIRefresher;
        private bool requiresUIUpdate = false;
        //\Private properties

        //Data hiding
        private int progressValue = 0;

        private int progressMaximum = 1;

        private bool scrollConsole = false;

        private string consoleInput = string.Empty;

        private ObservableCollection<string> consoleOutput = new ObservableCollection<string>();

        private string nuGetID;

        private string nuGetVersion;

        private List<string> nuGetIDAutocomplete = new List<string>();

        private string updateBranch = "master";
        //\Data hiding

        //Properties names
        private static readonly string ScrollConsolePropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ScrollConsole);

        private static readonly string ProgressValuePropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ProgressValue);

        private static readonly string ProgressMaximumPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ProgressMaximum);

        private static readonly string ConsoleInputPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ConsoleInput);

        private static readonly string ConsoleOutputPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ConsoleOutput);

        private static readonly string NuGetIDAutocompletePropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.NuGetIDAutocomplete);

        private static readonly string NuGetIDPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.NuGetID);

        private static readonly string NuGetVersionPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.NuGetVersion);

        private static readonly string UpdateBranchPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.UpdateBranch);
        //\Properties names

        //Commands
        public RelayCommand UpdateAllCommand { get; set; }

        public RelayCommand UpdateSelectedCommand { get; set; }

        public RelayCommand ConsoleReturn { get; set; }

        public RelayCommand RefreshBranchesCommand { get; set; }
        //\Commands

        //Bindable properties
        public GitReposViewModel GitReposVM { get; set; }

        public string UpdateBranch
        {
            get
            {
                return updateBranch;
            }
            set
            {
                updateBranch = value;
                OnPropertyChanged(UpdateBranchPropName);
            }
        }

        public bool ScrollConsole
        {
            get
            {
                return scrollConsole;
            }
            set
            {
                scrollConsole = value;
                OnPropertyChanged(ScrollConsolePropName);
            }
        }

        public int ProgressValue
        {
            get
            {
                return progressValue;
            }
            set
            {
                progressValue = value;
                OnPropertyChanged(ProgressValuePropName);
            }
        }

        public int ProgressMaximum
        {
            get
            {
                return progressMaximum;
            }
            set
            {
                progressMaximum = value;
                OnPropertyChanged(ProgressMaximumPropName);
            }
        }

        public string ConsoleInput
        {
            get
            {
                return consoleInput;
            }
            set
            {
                consoleInput = value;
                OnPropertyChanged(ConsoleInputPropName);
            }
        }

        public ObservableCollection<string> ConsoleOutput
        {
            get
            {
                return consoleOutput;
            }
            set
            {
                consoleOutput = value;
                OnPropertyChanged(ConsoleOutputPropName);
            }
        }

        public List<string> NuGetIDAutocomplete
        {
            get
            {
                return nuGetIDAutocomplete;
            }
            set
            {
                nuGetIDAutocomplete = value;
                OnPropertyChanged(NuGetIDAutocompletePropName);
            }
        }

        [SaveSceneAttribute]
        public string NuGetID
        {
            get
            {
                return nuGetID;
            }
            set
            {
                nuGetID = value;
                OnPropertyChanged(NuGetIDPropName);
                ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
            }
        }

        [SaveSceneAttribute]
        public string NuGetVersion
        {
            get
            {
                return nuGetVersion;
            }
            set
            {
                nuGetVersion = value;
                OnPropertyChanged(NuGetVersionPropName);
                ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
            }
        }
        //\Bindable properties

        //Implementation
        public UpdateViewModel()
        {
            UpdateAllCommand = new RelayCommand(UpdateAllExecute, UpdateAllCanExecute);
            UpdateSelectedCommand = new RelayCommand(UpdateSelectedExecute, UpdateSelectedCanExecute);
            ConsoleReturn = new RelayCommand(ConsoleReturnExecute, ConsoleReturnCanExecute);
            RefreshBranchesCommand = new RelayCommand(RefreshBranchesExecute, RefreshBranchesCanExecute);
            gitAdapter = new GitAdapterCore();
            uIRefresher = new TimerUpdater(new System.TimeSpan(0, 0, 1), () =>
            {
                if (requiresUIUpdate)
                {
                    ViewModelService.GetViewModel<GitReposViewModel>().UpdatePackages();
                    ViewModelService.GetViewModel<GitReposViewModel>().UpdateRepoBranches();
                    requiresUIUpdate = false;
                    OnPropertyChanged("UIUpdateCounter");
                }
            });
        }

        private void UpdateRepos(IEnumerable<GitRepoModel> gitReposModel)
        {
            calcRunning = true;
            ProgressValue = 0;
            var gitReposList = new List<GitRepoDTO>();
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;
            foreach (var gitRepoViewModel in gitReposModel)
            {
                gitReposList.Add(new GitRepoDTO()
                {
                    Password = gitRepoViewModel.UseOverrideCredentials ? gitRepoViewModel.Password : ViewModelService.GetViewModel<GitReposViewModel>().OverridenPassword,
                    URL = gitRepoViewModel.Url,
                    Username = gitRepoViewModel.UseOverrideCredentials ? gitRepoViewModel.Username : ViewModelService.GetViewModel<GitReposViewModel>().OveridenUsername,
                    Path = Path.Combine(basePath, gitRepoViewModel.RepoName),
                    UpdateBranch = gitRepoViewModel.UseUpdateBranch ? gitRepoViewModel.UpdateBranch : UpdateBranch
                });
            }
            ProgressMaximum = gitReposModel.Count();
            gitAdapter.UpdateProjectsDependencies(new UpdateProjectsDependenciesRequestDTO()
            {
                ListOfRepos = gitReposList,
                ProgressAction = ProgressCallback,
                FinishedAction = FinishedCallback,
                NugetID = NuGetID,
                Version = NuGetVersion
            });
            calcRunning = false;
        }

        public void UpdatePackages()
        {
            requiresUIUpdate = true;
        }

        public void Initialize()
        {
            GitReposVM = ViewModelService.GetViewModel<GitReposViewModel>();
        }
        //\Implementation

        //Commands logic
        private bool ConsoleReturnCanExecute(object arg)
        {
            if (calcRunning)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(consoleInput))
            {
                return true;
            }
            return false;
        }

        private void ConsoleReturnExecute(object obj)
        {
            consoleOutput.Add(consoleInput);
            ConsoleInput = string.Empty;
            ScrollConsole = true;
        }

        private bool UpdateSelectedCanExecute(object arg)
        {
            if (calcRunning)
            {
                return false;
            }
            var reposVM = ViewModelService.GetViewModel<GitReposViewModel>();
            if (reposVM.GitRepos.Count < 1)
            {
                return false;
            }
            if (reposVM.GitRepos.Where(a => a.UpdateToggle).Count() < 1)
            {
                return false;
            }
            return true;
        }

        private void UpdateSelectedExecute(object obj)
        {
            var reposVM = ViewModelService.GetViewModel<GitReposViewModel>();
            if (reposVM.GitRepos.Count > 0)
            {
                var reposToUpdate = reposVM.GitRepos.Where(a => a.UpdateToggle);
                if (reposToUpdate.Count() > 0)
                {
                    new Task(() => { UpdateRepos(reposToUpdate); }).Start();
                }
            }
        }

        private bool UpdateAllCanExecute(object arg)
        {
            if (calcRunning)
            {
                return false;
            }
            var reposVM = ViewModelService.GetViewModel<GitReposViewModel>();
            if (reposVM.GitRepos.Count < 1)
            {
                return false;
            }
            return true;
        }

        private void UpdateAllExecute(object obj)
        {
            var reposVM = ViewModelService.GetViewModel<GitReposViewModel>();
            if (reposVM.GitRepos.Count > 0)
            {
                new Task(() => { UpdateRepos(reposVM.GitRepos); }).Start();
            }
        }

        private bool RefreshBranchesCanExecute(object arg)
        {
            var reposVM = ViewModelService.GetViewModel<GitReposViewModel>();
            if (reposVM.GitRepos.Count > 0)
            {
                var updateAbleRepos = reposVM.GitRepos.Select(a=>a.CloneStatus==CloneStatusEnum.AlreadyCloned);
                if (updateAbleRepos.Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void RefreshBranchesExecute(object obj)
        {
            ViewModelService.GetViewModel<GitReposViewModel>().UpdateRepoBranches();
        }
        //\Commands logic

        //Callbacks
        public void FinishedCallback(string consoleMessage)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    ConsoleOutput.Add(consoleMessage);
                    ScrollConsole = true;
                    ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
                    ViewModelService.GetViewModel<GitReposViewModel>().RefreshBindings();
                    CommandManager.InvalidateRequerySuggested();
                }));
        }

        public void ProgressCallback(bool progress, string consoleMessage)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    ConsoleOutput.Add(consoleMessage);
                    if (progress)
                    {
                        ProgressValue++;
                        ScrollConsole = true;
                    }
                    ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
                    ViewModelService.GetViewModel<GitReposViewModel>().RefreshBindings();
                }));
        }
        //\Callbacks
    }
}
