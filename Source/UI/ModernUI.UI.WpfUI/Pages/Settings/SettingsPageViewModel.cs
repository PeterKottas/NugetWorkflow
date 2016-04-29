using FirstFloor.ModernUI.Presentation;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Base;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetWorkflow.UI.WpfUI.Pages.Settings
{
    public class SettingsPageViewModel : BaseViewModel, IViewModel
    {
        //Private properties
        private static HomePageViewModel homeVm
        {
            get
            {
                return ViewModelService.GetViewModel<HomePageViewModel>();
            }
        }
        //\Private properties

        //Commands
        public RelayCommand SaveConfigCommand { get; set; }

        public RelayCommand LoadConfigCommand { get; set; }
        //\Commands

        //Implementation
        public SettingsPageViewModel()
        {
            SaveConfigCommand = new RelayCommand(SaveConfigExecute, SaveConfigCanExecute);
            LoadConfigCommand = new RelayCommand(LoadConfigExecute);
        }

        private void LoadConfigExecute(object obj)
        {
            ConfigSaver.OpenUI();
        }

        private bool SaveConfigCanExecute(object arg)
        {
            return homeVm.IsConfigDirty;
        }

        private void SaveConfigExecute(object obj)
        {
            ConfigSaver.Save();
        }

        public void Initialize()
        {
        }
        //\Implementation
    }
}
