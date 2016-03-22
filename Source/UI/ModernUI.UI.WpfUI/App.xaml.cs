using NugetWorkflow.UI.WpfUI.Common.Exceptions;
using NugetWorkflow.UI.WpfUI.Common.Interfaces;
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
        private Dictionary<Type, object> viewDictionary;
        public RET GetView<RET>()
        {
            var contains = viewDictionary.Keys.Contains(typeof(RET));
            if (!contains)
            {
                throw new MissingViewException(string.Format("{0} is missing or does not implement {1} interface.", typeof(RET).FullName, typeof(IView).FullName));
            }
            return (RET)viewDictionary[typeof(RET)];
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            SetupViewDictionary();
        }

        private void SetupViewDictionary()
        {
            viewDictionary = new Dictionary<Type, object>();
            var viewInterface = typeof(IView);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => viewInterface.IsAssignableFrom(p) && p != viewInterface);
            foreach (var type in types)
            {
                viewDictionary.Add(type, Activator.CreateInstance(type));
            }
        }
    }
}
