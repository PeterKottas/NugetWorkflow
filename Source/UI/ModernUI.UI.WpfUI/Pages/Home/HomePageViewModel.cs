using FirstFloor.ModernUI.Presentation;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Attributes;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    public class HomePageViewModel : BaseViewModel, IViewModel
    {
        //Private properties
        private Dictionary<string, string> headers = new Dictionary<string, string>()
            {
                {"/Pages/Home/SubHome/BaseSetup/BaseSetupPage.xaml","You can do the base setup here"},
                {"/Pages/Home/SubHome/GitRepos/GitReposPage.xaml","Now lets clone some serious code!"},
                {"/Pages/Home/SubHome/Clone/ClonePage.xaml","Setup your git server connections"},
                {"/Pages/Home/SubHome/Update/UpdatePage.xaml","Update your NuGet dependencies here"}
            };
        private bool isDirty = false;
        private string header = null;
        private string selectedPage = "/Pages/Home/SubHome/BaseSetup/BaseSetupPage.xaml";
        //\Private properties

        //Properties names        
        public static readonly string HeaderPropName = ReflectionUtility.GetPropertyName((HomePageViewModel s) => s.Header);
        public static readonly string IsDirtyPropName = ReflectionUtility.GetPropertyName((HomePageViewModel s) => s.IsDirty);
        public static readonly string SelectedPagePropName = ReflectionUtility.GetPropertyName((HomePageViewModel s) => s.SelectedPage);
        //\Properties names

        //Bindable properties
        [SaveSceneAttribute]
        public string SelectedPage
        {
            get
            {
                return selectedPage;
            }
            set
            {
                selectedPage = value;
                OnPropertyChanged(IsDirtyPropName);
            }
        }

        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                isDirty = value;
                OnPropertyChanged(IsDirtyPropName);
            }
        }

        public string Header
        {
            get
            {
                return header;
            }
            set
            {
                header = value;
                OnPropertyChanged(HeaderPropName);
            }
        }
        //\Bindable properties

        //Commands
        public RelayCommand NewFileCommand { get; set; }

        public RelayCommand SaveFileCommand { get; set; }

        public RelayCommand OpenFileCommand { get; set; }

        public RelayCommand SaveAsFileCommand { get; set; }

        public RelayCommand ExitCommand { get; set; }

        public RelayCommand UndoCommand { get; set; }

        public RelayCommand RedoCommand { get; set; }
        //\Commands

        //Implementation
        public HomePageViewModel()
        {
            NewFileCommand = new RelayCommand(NewFileExecute, NewFileCanExecute);
            SaveFileCommand = new RelayCommand(SaveFileExecute, SaveFileCanExecute);
            OpenFileCommand = new RelayCommand(OpenFileExecute, OpenFileCanExecute);
            SaveAsFileCommand = new RelayCommand(SaveAsExecute, SaveAsCanExecute);
            ExitCommand = new RelayCommand(ExitExecute, ExitExecuteCanExecute);
            UndoCommand = new RelayCommand(UndoExecute, UndoCanExecute);
            RedoCommand = new RelayCommand(RedoExecute, RedoCanExecute);
        }

        private bool RedoCanExecute(object arg)
        {
            return UndoManager.RedoCanExecute();
        }

        private void RedoExecute(object obj)
        {
            UndoManager.Redo();
        }

        private bool UndoCanExecute(object arg)
        {
            return UndoManager.UndoCanExecute();
        }

        private void UndoExecute(object obj)
        {
            UndoManager.Undo();
        }

        private bool ExitExecuteCanExecute(object arg)
        {
            return true;
        }

        private void ExitExecute(object obj)
        {
            Application.Current.Shutdown();
        }

        public void Initialize()
        {
        }

        public void UpdateHeader(string key)
        {
            if(headers.ContainsKey(key))
            {
                Header = headers[key];
            }
        }
        //\Implementation

        //Commands logic
        private bool SaveAsCanExecute(object arg)
        {
            if (IsDirty)
            {
                return true;
            }
            return false;
        }

        private void SaveAsExecute(object obj)
        {
            SceneSaver.SaveUI();
        }

        private bool OpenFileCanExecute(object arg)
        {
            return true;
        }

        private void OpenFileExecute(object obj)
        {
            SceneSaver.OpenUI();
        }

        private bool SaveFileCanExecute(object arg)
        {
            if (IsDirty)
            {
                return true;
            }
            return false;
        }

        private void SaveFileExecute(object obj)
        {
            SceneSaver.Save();
        }

        private bool NewFileCanExecute(object arg)
        {
            return true;
        }

        private void NewFileExecute(object obj)
        {
            if (SceneSaver.HandleDirtySceneOverWrite())
            {
                ViewModelService.SetupViewDictionary();
                PageUserControlService.ReassignViewModels();
                SceneSaver.MakeClean();
            }
        }

        private void ExitCommandExecute(object obj)
        {
            Application.Current.Shutdown();
        }
        //\Commands logic
    }
}
