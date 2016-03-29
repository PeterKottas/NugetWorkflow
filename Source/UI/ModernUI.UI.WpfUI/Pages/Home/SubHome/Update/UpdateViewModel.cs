using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.DTOs.GitRepos;
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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Update
{
    public class UpdateViewModel : NotifyPropertyChanged, IViewModel
    {
        private bool calcRunning = false;
        private IGitAdapter gitAdapter;
        private int progressValue = 0;
        private int progressMaximum = 1;
        private bool scrollConsole = false;
        private string consoleInput = string.Empty;
        private ObservableCollection<string> consoleOutput = new ObservableCollection<string>() { "This is how you emulate a freaking console in WPF :p" };

        #region Properties names
        public static readonly string ScrollConsolePropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ScrollConsole);
        public static readonly string ProgressValuePropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ProgressValue);
        public static readonly string ProgressMaximumPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ProgressMaximum);
        public static readonly string ConsoleInputPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ConsoleInput);
        public static readonly string ConsoleOutputPropName = ReflectionUtility.GetPropertyName((UpdateViewModel s) => s.ConsoleOutput);
        #endregion

        public RelayCommand UpdateAllCommand { get; set; }
        public RelayCommand UpdateSelectedCommand { get; set; }
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

        public UpdateViewModel()
        {
            UpdateAllCommand = new RelayCommand(UpdateAllExecute, UpdateAllCanExecute);
            UpdateSelectedCommand = new RelayCommand(UpdateSelectedExecute, UpdateSelectedCanExecute);
            ConsoleReturn = new RelayCommand(ConsoleReturnExecute, ConsoleReturnCanExecute);
            gitAdapter = new GitAdapterCore();
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

        public void UpdateRepos(IEnumerable<GitRepoModel> gitReposModel)
        {
            calcRunning = true;
            ProgressValue = 0;
            ProgressMaximum = gitReposModel.Count();
            var gitReposList = new List<GitRepoDTO>();
            foreach (var gitRepoViewModel in gitReposModel)
            {
                gitReposList.Add(new GitRepoDTO()
                    {
                        Password = gitRepoViewModel.UseOverrideCredentials ? gitRepoViewModel.Password : ViewModelService.GetViewModel<GitReposViewModel>().OverridenPassword,
                        URL = gitRepoViewModel.Url,
                        Username = gitRepoViewModel.UseOverrideCredentials ? gitRepoViewModel.Username : ViewModelService.GetViewModel<GitReposViewModel>().OveridenUsername
                    });
            }
            gitAdapter.UpdateProjectsDependencies(new UpdateProjectsDependenciesRequestDTO()
                {
                    BasePath = ViewModelService.GetViewModel<BaseSetupViewModel>().BasePath,
                    ListOfRepos = gitReposList,
                    ProgressAction = ProgressCallback
                });
            calcRunning = false;
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Normal,
                (Action)(() =>
                {
                    ScrollConsole = true;
                    ViewModelService.GetViewModel<GitReposViewModel>().RefreshBindings();
                    CommandManager.InvalidateRequerySuggested();
                }));
        }

        private void UpdateAllExecute(object obj)
        {
            var reposVM = ViewModelService.GetViewModel<GitReposViewModel>();
            if (reposVM.GitRepos.Count > 0)
            {
                new Task(() => { UpdateRepos(reposVM.GitRepos); }).Start();
            }
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
