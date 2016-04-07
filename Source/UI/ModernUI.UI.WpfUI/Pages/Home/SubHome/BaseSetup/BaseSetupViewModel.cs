using FirstFloor.ModernUI.Presentation;
using Microsoft.WindowsAPICodePack.Dialogs;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Attributes;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.Update;
using NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.General;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.IO;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup
{
    [SaveSceneAttribute]
    public class BaseSetupViewModel : BaseViewModel, IViewModel
    {
        //Data hiding
        private string basePath = @"C:\NugetTest";
        //private string basePath = @"C:\Users\MasterPC\Desktop\Delete";
        //\Data hiding

        //Properties names
        private static readonly string BasePathPropName = ReflectionUtility.GetPropertyName((BaseSetupViewModel s) => s.BasePath);
        //\Properties names        

        //Commands
        public RelayCommand ChooseBasePathCommand { get; set; }
        //\Commands

        //Bindable properties
        [SaveSceneAttribute]
        public string BasePath
        {
            get
            {
                return basePath;
            }
            set
            {
                var orig = basePath;
                OnUndoRedoPropertyChanged(BasePathPropName, 
                    () => 
                    {
                        ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
                        ViewModelService.GetViewModel<UpdateViewModel>().UpdatePackages();
                        basePath = orig;
                        ViewModelService.GetViewModel<GeneralSettingsViewModel>().UpdateBasePath(value);
                    }, 
                    () => 
                    {
                        ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
                        ViewModelService.GetViewModel<UpdateViewModel>().UpdatePackages();
                        basePath = value;
                        ViewModelService.GetViewModel<GeneralSettingsViewModel>().UpdateBasePath(value);
                    });
            }
        }
        //\Bindable properties

        //Implementation
        public BaseSetupViewModel()
        {
            ChooseBasePathCommand = new RelayCommand(ChooseBasePathExecute);
        }

        public void Initialize()
        {
            ViewModelService.GetViewModel<GitReposViewModel>().UpdateStatuses();
            ViewModelService.GetViewModel<UpdateViewModel>().UpdatePackages();
            ViewModelService.GetViewModel<GeneralSettingsViewModel>().UpdateBasePath(BasePath);
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
