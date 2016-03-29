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
        private string basePath = @"C:\Users\peter.kottas\Desktop\Delete";

        #region Properties names
        public static readonly string BasePathPropName = ReflectionUtility.GetPropertyName((BaseSetupViewModel s) => s.BasePath);
        #endregion

        public RelayCommand ChooseBasePathCommand { get; set; }

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

        public BaseSetupViewModel()
        {
            ChooseBasePathCommand = new RelayCommand(ChooseBasePathExecute);
        }

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
    }
}
