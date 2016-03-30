using FirstFloor.ModernUI.Presentation;
using Microsoft.WindowsAPICodePack.Dialogs;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.GitRepos;
using NugetWorkflow.UI.WpfUI.Utils;

namespace NugetWorkflow.UI.WpfUI.Pages.Home.SubHome.BaseSetup
{
    public class BaseSetupViewModel : NotifyPropertyChanged, IViewModel
    {
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
                ViewModelService.GetViewModel<GitReposViewModel>().UpdateBasePath();
                OnPropertyChanged(BasePathPropName);
            }
        }
        //\Bindable properties

        //Implementation
        public BaseSetupViewModel()
        {
            ChooseBasePathCommand = new RelayCommand(ChooseBasePathExecute);
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
