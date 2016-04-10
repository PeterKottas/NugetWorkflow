using Microsoft.Win32;
using NugetWorkflow.Common.Base.BaseClasses;
using NugetWorkflow.Common.Base.Exceptions;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
            SceneSaver.MakeClean();
            UndoManager.ResetBuffer();
        }
    }
}
