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
        private bool autoLoadSceneIsEnabled;
        private bool autoLoadLastSavedIsEnabled;
        private string lastSavedScene;
        private string chosenSceneToAutoLoad;
        //\Private properties

        //Data hiding
        private bool fileWatcherIsEnabled = true;
        //\Data hiding

        //Properties names        
        private static readonly string FileWatcherIsEnabledPropName = ReflectionUtility.GetPropertyName((GeneralSettingsViewModel s) => s.FileWatcherIsEnabled);
        private static readonly string UndoRedoLimitPropName = ReflectionUtility.GetPropertyName((GeneralSettingsViewModel s) => s.UndoRedoLimit);
        private static readonly string AutoLoadSceneIsEnabledPropName = ReflectionUtility.GetPropertyName((GeneralSettingsViewModel s) => s.AutoLoadSceneIsEnabled);
        private static readonly string AutoLoadLastSavedIsEnabledPropName = ReflectionUtility.GetPropertyName((GeneralSettingsViewModel s) => s.AutoLoadLastSavedIsEnabled);
        private static readonly string SceneToAutoLoadPropName = ReflectionUtility.GetPropertyName((GeneralSettingsViewModel s) => s.SceneToAutoLoad);
        private static readonly string LastSavedScenePropName = ReflectionUtility.GetPropertyName((GeneralSettingsViewModel s) => s.LastSavedScene);
        private static readonly string ChosenSceneToAutoLoadPropName = ReflectionUtility.GetPropertyName((GeneralSettingsViewModel s) => s.ChosenSceneToAutoLoad);
        //\Properties names        

        //Bindable properties
        [SaveConfigAttribute]
        public int UndoRedoLimit
        {
            get
            {
                return UndoManager.UndoRedoLimit;
            }
            set
            {
                UndoManager.UndoRedoLimit = value;
                OnPropertyChanged(UndoRedoLimitPropName);
            }
        }

        [SaveConfigAttribute]
        public bool AutoLoadSceneIsEnabled
        {
            get
            {
                return autoLoadSceneIsEnabled;
            }
            set
            {
                autoLoadSceneIsEnabled = value;
                OnPropertyChanged(AutoLoadSceneIsEnabledPropName);
            }
        }

        [SaveConfigAttribute]
        public bool AutoLoadLastSavedIsEnabled
        {
            get
            {
                return autoLoadLastSavedIsEnabled;
            }
            set
            {
                autoLoadLastSavedIsEnabled = value;
                OnPropertyChanged(AutoLoadLastSavedIsEnabledPropName);
                OnPropertyChanged(SceneToAutoLoadPropName);
            }
        }

        [SaveConfigAttribute]
        public string LastSavedScene
        {
            get
            {
                return lastSavedScene;
            }
            set
            {
                lastSavedScene = value;
                OnPropertyChanged(LastSavedScenePropName);
                OnPropertyChanged(SceneToAutoLoadPropName);
            }
        }

        [SaveConfigAttribute]
        public string ChosenSceneToAutoLoad
        {
            get
            {
                return chosenSceneToAutoLoad;
            }
            set
            {
                chosenSceneToAutoLoad = value;
                OnPropertyChanged(ChosenSceneToAutoLoadPropName);
                OnPropertyChanged(SceneToAutoLoadPropName);
            }
        }

        public string SceneToAutoLoad
        {
            get
            {
                if (AutoLoadLastSavedIsEnabled)
                {
                    return LastSavedScene;
                }
                else
                {
                    return ChosenSceneToAutoLoad;
                }
            }
            set
            {
                if (!AutoLoadLastSavedIsEnabled)
                {
                    ChosenSceneToAutoLoad = value;
                }
            }
        }

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

        public bool UndoRedoEnabled
        {
            get
            {
                return UndoManager.IsEnabled;
            }
            set
            {
                UndoManager.IsEnabled = value;
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
            AutoLoadSceneIsEnabled = true;
            AutoLoadLastSavedIsEnabled = true;
        }
    }
}
