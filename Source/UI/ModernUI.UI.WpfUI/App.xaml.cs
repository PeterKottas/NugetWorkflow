using Autofac;
using NugetWorkflow.UI.WpfUI.Pages.CLoneProjects;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IContainer Container { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var builder = new ContainerBuilder();
            builder.RegisterType<HomePageViewModel>().SingleInstance();
            builder.RegisterType<CloneProjectsViewModel>().SingleInstance();
            Container = builder.Build();
        }
    }
}
