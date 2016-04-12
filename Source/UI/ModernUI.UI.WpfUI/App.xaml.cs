using Microsoft.Win32;
using NugetWorkflow.Common.Base.BaseClasses;
using NugetWorkflow.Common.Base.Exceptions;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Pages.Home;
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
            /*var task = new Task(update);
            task.Start();
            task.Wait();*/

            ViewModelService.SetupViewDictionary();
            SceneSaver.MakeClean();
            UndoManager.ResetBuffer();
        }

        private async void update()
        {
            try
            {
                using (var mgr = new UpdateManager(@"http://ec2-52-16-197-231.eu-west-1.compute.amazonaws.com:1234"))
                {
                    await mgr.UpdateApp();
                }
            }
            catch (Exception exception)
            {
                //MessageBox.Show("Exception while updating : " + exception.Message);
            }
        }
    }
}
