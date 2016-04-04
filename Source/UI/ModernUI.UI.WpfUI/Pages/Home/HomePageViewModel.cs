using FirstFloor.ModernUI.Presentation;
using Microsoft.WindowsAPICodePack.Dialogs;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.Common.Base.Utils;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Home
{
    public class HomePageViewModel : BaseViewModel, IViewModel
    {
        //Private properties
        private string header;
        private bool isDirty = false;
        //\Private properties

        //Properties names        
        public static readonly string HeaderPropName = ReflectionUtility.GetPropertyName((HomePageViewModel s) => s.Header);
        public static readonly string IsDirtyPropName = ReflectionUtility.GetPropertyName((HomePageViewModel s) => s.IsDirty);
        //\Properties names

        //Bindable properties
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
        //\Commands

        //Implementation
        public HomePageViewModel()
        {
            NewFileCommand = new RelayCommand(NewFileExecute, NewFileCanExecute);
            SaveFileCommand = new RelayCommand(SaveFileExecute, SaveFileCanExecute);
            OpenFileCommand = new RelayCommand(OpenFileExecute, OpenFileCanExecute);
            SaveAsFileCommand = new RelayCommand(SaveAsExecute, SaveAsCanExecute);
        }

        public void Initialize()
        {
        }
        //\Implementation

        //Commands logic
        private bool SaveAsCanExecute(object arg)
        {
            if(IsDirty)
            {
                return true;
            }
            return false;
        }

        private void SaveAsExecute(object obj)
        {
            var dlg = new CommonSaveFileDialog();
            dlg.Title = "Save your current scene";
            dlg.AddToMostRecentlyUsedList = false;
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.ShowPlacesList = true;
            dlg.Filters.Add(new CommonFileDialogFilter("Scene","scn"));
            dlg.DefaultExtension = "scn";

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SceneSaver.Save(dlg.FileName);
            }
            dlg.Dispose();
            SceneSaver.MakeClean();
        }

        private bool OpenFileCanExecute(object arg)
        {
            return true;
        }

        private void OpenFileExecute(object obj)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Choose scene to open";
            dlg.IsFolderPicker = false;
            dlg.Multiselect = false;
            dlg.Filters.Add(new CommonFileDialogFilter("Scene", "scn"));
            dlg.DefaultExtension = "scn";

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SceneSaver.Load(dlg.FileName);
            }
            dlg.Dispose();
            SceneSaver.MakeClean();
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
            
        }

        private bool NewFileCanExecute(object arg)
        {
            if (IsDirty)
            {
                return true;
            }
            return false;
        }

        private void NewFileExecute(object obj)
        {
            IsDirty = false;
        }
        //\Commands logic
    }
}
