using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Attributes;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Update;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.General
{
    [SaveConfigAttribute]
    public class GeneralSettingsViewModel : BaseViewModel, IViewModel, IDisposable
    {
        //Private properties
        private FileSystemWatcher basePathWatcher;
        private TimerUpdater UIRefresher;
        private bool requiresUIUpdate;
        //\Private properties

        //Data hiding
        private bool fileWatcherIsEnabled = true;
        //\Data hiding

        //Properties names        
        private static readonly string FileWatcherIsEnabledPropName = ReflectionUtility.GetPropertyName((GeneralSettingsViewModel s) => s.FileWatcherIsEnabled);
        //\Properties names        

        //Bindable properties
        public int RenameCounter { get; set; }

        public int ChangedCounter { get; set; }

        public int UIUpdateCounter { get; set; }

        [SaveConfigAttribute]
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
            }
        }
        //\Bindable properties

        //Implementation
        public GeneralSettingsViewModel()
        {
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
                if (requiresUIUpdate)
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

        public void UpdateBasePath(string BasePath)
        {
            if (FileWatcherIsEnabled)
            {
                if (Directory.Exists(BasePath))
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

        public void Dispose()
        {
            basePathWatcher.Dispose();
        }
        //\Implementation

        public void Initialize()
        {
        }
    }
}
