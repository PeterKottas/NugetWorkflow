using NugetWorkflow.Common.Base.BaseClasses;
using NugetWorkflow.Common.Base.Exceptions;
using NugetWorkflow.Common.Base.Interfaces;
using NugetWorkflow.UI.WpfUI.Pages.Home;
using NugetWorkflow.UI.WpfUI.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Dictionary<Type, object> viewDictionary;

        public Dictionary<Type, object> ViewDictionary
        {
            get
            {
                return viewDictionary;
            }
            set
            {
                viewDictionary = value;
            }
        }

        private void SetupViewDictionary()
        {
            viewDictionary = new Dictionary<Type, object>();
            var viewInterface = typeof(IViewModel);
            var types = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.StartsWith("NugetWorkflow.UI.WpfUI")).SelectMany(s => s.GetTypes())
                .Where(p => viewInterface.IsAssignableFrom(p) && p != viewInterface);
            foreach (var type in types)
            {
                viewDictionary.Add(type, Activator.CreateInstance(type));
            }
            foreach (IViewModel view in viewDictionary.Values)
            {
                view.Initialize();
            }
        }

        public RET GetView<RET>()
        {
            return (RET)GetView(typeof(RET));
        }

        public object GetView(Type type)
        {
            var contains = viewDictionary.Keys.Contains(type);
            if (!contains)
            {
                throw new MissingViewException(string.Format("{0} is missing or does not implement {1} interface.", type.FullName, typeof(IViewModel).FullName));
            }
            return viewDictionary[type];
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            SetupViewDictionary();
            //SceneSaver.Save("test");
            SceneSaver.MakeClean();
        }
    }
}
