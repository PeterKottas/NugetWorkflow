using FirstFloor.ModernUI.Presentation;
using Microsoft.WindowsAPICodePack.Dialogs;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Utils;
using System.IO;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup
{
    public class BaseSetupViewModel : NotifyPropertyChanged, IViewModel
    {
        //Private properties
        private FileSystemWatcher basePathWatcher;
        //\Private properties

        //Data hiding
        private string basePath = @"C:\Users\peter.kottas\Desktop\Delete";
        //\Data hiding

        //Properties names
        private static readonly string BasePathPropName = ReflectionUtility.GetPropertyName((BaseSetupViewModel s) => s.BasePath);
        //\Properties names        

        //Commands
        public RelayCommand ChooseBasePathCommand { get; set; }
        //\Commands

        //Bindable properties
        public string BasePath
        {
            get
            {
                return basePath;
            }
            set
            {
                basePath = value;
                ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
                OnPropertyChanged(BasePathPropName);
                if (Directory.Exists(basePath))
                {
                    basePathWatcher.Path = BasePath;
                    if (!basePathWatcher.EnableRaisingEvents)
                    {
                        basePathWatcher.EnableRaisingEvents = true;
                    }
                }
                else
                {
                    basePathWatcher.EnableRaisingEvents = false;
                }
            }
        }
        //\Bindable properties

        //Implementation
        public BaseSetupViewModel()
        {
            ChooseBasePathCommand = new RelayCommand(ChooseBasePathExecute);
            basePathWatcher = new FileSystemWatcher();
            basePathWatcher.IncludeSubdirectories = true;
            basePathWatcher.NotifyFilter = NotifyFilters.LastAccess |
                         NotifyFilters.LastWrite |
                         NotifyFilters.FileName |
                         NotifyFilters.DirectoryName;
            basePathWatcher.Changed += OnChanged;
            basePathWatcher.Deleted += OnChanged;
            basePathWatcher.Created += OnChanged;
            basePathWatcher.Renamed += OnRename;
            RenameCounter = 0;
            ChangedCounter = 0;
        }

        public int RenameCounter { get; set; }

        public int ChangedCounter { get; set; }

        void OnRename(object sender, RenamedEventArgs e)
        {
            ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
            RenameCounter++;
            OnPropertyChanged("RenameCounter");
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
            ChangedCounter++;
            OnPropertyChanged("ChangedCounter");
        }
        //Implementation

        //Commands logic
        private void ChooseBasePathExecute(object parameter)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Choose base path for your nuget solutions";
            dlg.IsFolderPicker = true;

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                BasePath = dlg.FileName;
            }
        }
        //\Commands logic
    }
}
