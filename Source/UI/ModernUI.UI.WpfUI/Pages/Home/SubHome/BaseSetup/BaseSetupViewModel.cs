using FirstFloor.ModernUI.Presentation;
using Microsoft.WindowsAPICodePack.Dialogs;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Update;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.IO;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup
{
    public class BaseSetupViewModel : NotifyPropertyChanged, IViewModel, IDisposable
    {
        //Private properties
        private FileSystemWatcher basePathWatcher;
        private TimerUpdater UIRefresher;
        private bool requiresUIUpdate;
        //\Private properties

        //Data hiding
        private string basePath = @"C:\Users\peter.kottas\Desktop\Delete";
        //private string basePath = @"C:\Users\MasterPC\Desktop\Delete";
        private bool fileWatcherIsEnabled = true;
        //\Data hiding

        //Properties names
        private static readonly string BasePathPropName = ReflectionUtility.GetPropertyName((BaseSetupViewModel s) => s.BasePath);
        private static readonly string FileWatcherIsEnabledPropName = ReflectionUtility.GetPropertyName((BaseSetupViewModel s) => s.FileWatcherIsEnabled);
        //\Properties names        

        //Commands
        public RelayCommand ChooseBasePathCommand { get; set; }
        //\Commands

        //Bindable properties
        public bool FileWatcherIsEnabled
        {
            get
            {
                return fileWatcherIsEnabled;
            }
            set
            {
                fileWatcherIsEnabled = value;
                OnPropertyChanged(FileWatcherIsEnabledPropName);
                BasePath = BasePath;
            }
        }

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
                ViewModelService.GetViewModel<UpdateViewModel>().UpdatePackages();
                OnPropertyChanged(BasePathPropName);
                if (FileWatcherIsEnabled)
                {
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
                else
                {
                    basePathWatcher.EnableRaisingEvents = false;
                }
            }
        }

        public int RenameCounter { get; set; }

        public int ChangedCounter { get; set; }

        public int UIUpdateCounter { get; set; }
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
            UIRefresher = new TimerUpdater(new System.TimeSpan(0, 0, 1), () =>
            {
                if(requiresUIUpdate)
                {
                    ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
                    requiresUIUpdate = false;
                    UIUpdateCounter++;
                    OnPropertyChanged("UIUpdateCounter");
                }
            });
        }

        void OnRename(object sender, RenamedEventArgs e)
        {
            FileChanged(e.FullPath);
            RenameCounter++;
            OnPropertyChanged("RenameCounter");
        }

        void OnChanged(object sender, FileSystemEventArgs e)
        {
            FileChanged(e.FullPath);
            ChangedCounter++;
            OnPropertyChanged("ChangedCounter");
        }

        private void FileChanged(string filename)
        {
            requiresUIUpdate = true;
            if (Path.GetFileName(filename) == "packages.config")
            {
                ViewModelService.GetViewModel<UpdateViewModel>().UpdatePackages();
            }
        }

        public void Initialize()
        {
            BasePath = BasePath;
        }

        public void Dispose()
        {
            basePathWatcher.Dispose();
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
