using Microsoft.Win32;
using NugetWorkflow.Common.Base.BaseClasses;
using NugetWorkflow.Common.Base.Exceptions;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using NugetWorkflow.UI.WpfUI.Pages.Settings.SubSettings.Update;
using NugetWorkflow.UI.WpfUI.Utils;
using Squirrel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ViewModelService.SetupViewDictionary();
            UndoManager.Disable();
            ConfigSaver.Load();
            ConfigSaver.LoadLastSavedScene();
            SceneSaver.AutoLoad();
            UndoManager.ResetBuffer();
            SceneSaver.MakeClean();
            ConfigSaver.MakeClean();
            UndoManager.Enable();
            ViewModelService.GetViewModel<UpdateViewModel>().UpdateUI();
        }
    }
}
