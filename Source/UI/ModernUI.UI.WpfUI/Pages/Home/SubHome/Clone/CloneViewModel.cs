using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.DTOs.GitRepos;
using NugetWorkflow.Common.Base.Extensions;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.Common.GitAdapter.DTOs.Requests;
using NugetWorkflow.Common.GitAdapter.Interfaces;
using NugetWorkflow.Plugins.GitAdapter;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos.Models;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Clone
{
    public class CloneViewModel : NotifyPropertyChanged, IViewModel
    {
        private IGitAdapter gitAdapter;
        private int progressValue = 0;
        private int progressMaximum = 1;
        private bool scrollConsole = false;
        private string consoleInput = string.Empty;
        private ObservableCollection<string> consoleOutput = new ObservableCollection<string>() { "This is how you emulate a freaking console in WPF :p" };
        private bool calcRunning;

        #region Properties names
        public static readonly string ScrollConsolePropName = ReflectionUtility.GetPropertyName((CloneViewModel s) => s.ScrollConsole);
        public static readonly string ProgressValuePropName = ReflectionUtility.GetPropertyName((CloneViewModel s) => s.ProgressValue);
        public static readonly string ProgressMaximumPropName = ReflectionUtility.GetPropertyName((CloneViewModel s) => s.ProgressMaximum);
        public static readonly string ConsoleInputPropName = ReflectionUtility.GetPropertyName((CloneViewModel s) => s.ConsoleInput);
        public static readonly string ConsoleOutputPropName = ReflectionUtility.GetPropertyName((CloneViewModel s) => s.ConsoleOutput);
        #endregion

        public RelayCommand CloneAllCommand { get; set; }
        public RelayCommand CloneSelectedCommand { get; set; }
        public RelayCommand ConsoleReturn { get; set; }

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

        public CloneViewModel()
        {
            CloneAllCommand = new RelayCommand(CloneAllExecute, CloneAllCanExecute);
            CloneSelectedCommand = new RelayCommand(CloneSelectedExecute, CloneSelectedCanExecute);
            ConsoleReturn = new RelayCommand(ConsoleReturnExecute, ConsoleReturnCanExecute);
            gitAdapter = new GitAdapterCore();
            calcRunning = false;
        }

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

        private bool CloneSelectedCanExecute(object arg)
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
            if (reposVM.GitRepos.Where(a => a.CloneToggle).Count() < 1)
            {
                return false;
            }
            return true;
        }

        private void CloneSelectedExecute(object obj)
        {
            var reposVM = ViewModelService.GetViewModel<GitReposViewModel>();
            if (reposVM.GitRepos.Count > 0)
            {
                var reposToClone = reposVM.GitRepos.Where(a => a.CloneToggle);
                if (reposToClone.Count() > 0)
                {
                    new Task(() => { CloneRepos(reposToClone); }).Start();
                }
            }
        }

        private bool CloneAllCanExecute(object arg)
        {
            if(calcRunning)
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

        public void CloneRepos(IEnumerable<GitRepoModel> gitReposModel)
        {
            
            var gitReposList = new List<GitRepoDTO>();
            var basePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath;

            foreach (var gitRepoViewModel in gitReposModel)
            {
                if (gitRepoViewModel.CloneStatus == GitRepos.Shared.Enums.CloneStatusEnum.OK)
                {
                    gitReposList.Add(new GitRepoDTO()
                        {
                            Password = gitRepoViewModel.UseOverrideCredentials ? gitRepoViewModel.Password : ViewModelService.GetViewModel<GitReposViewModel>().OverridenPassword,
                            URL = gitRepoViewModel.Url,
                            Username = gitRepoViewModel.UseOverrideCredentials ? gitRepoViewModel.Username : ViewModelService.GetViewModel<GitReposViewModel>().OveridenUsername,
                            Path = Path.Combine(basePath, gitRepoViewModel.RepoName)
                        });
                }
            }
            calcRunning = true;
            ProgressValue = 0;
            ProgressMaximum = gitReposList.Count();
            gitAdapter.CloneProjects(new CloneProjectsRequestDTO()
                {
                    ListOfRepos = gitReposList,
                    ProgressAction = ProgressCallback,
                    FinishedAction = FinishedCallback
                });
        }

        private void CloneAllExecute(object obj)
        {
            var reposVM = ViewModelService.GetViewModel<GitReposViewModel>();
            if (reposVM.GitRepos.Count > 0)
            {
                new Task(() => { CloneRepos(reposVM.GitRepos); }).Start();
            }
        }

        public void FinishedCallback(string consoleMessage)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    ConsoleOutput.Add(consoleMessage);
                    ScrollConsole = true;
                    calcRunning = false;
                    ViewModelService.GetViewModel<GitReposViewModel>().RefreshBindings();
                    CommandManager.InvalidateRequerySuggested();
                }));
        }

        public void ProgressCallback(bool success, string consoleMessage)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    ConsoleOutput.Add(consoleMessage);
                    ProgressValue++;
                    ScrollConsole = true;
                    ViewModelService.GetViewModel<GitReposViewModel>().RefreshBindings();
                }));
        }
    }
}
