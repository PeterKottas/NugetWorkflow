using NugetWorkflow.Common.Base.Exceptions;
using NugetWorkflow.Common.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public static class ViewModelService
    {
        private static Dictionary<Type, object> viewDictionary;

        public static Dictionary<Type, object> ViewDictionary
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

        public static void SetupViewDictionary()
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

        public static RET GetViewModel<RET>()
        {
            return (RET)GetViewModel(typeof(RET));
        }

        public static object GetViewModel(Type type)
        {
            var contains = viewDictionary.Keys.Contains(type);
            if (!contains)
            {
                throw new MissingViewException(string.Format("{0} is missing or does not implement {1} interface.", type.FullName, typeof(IViewModel).FullName));
            }
            return viewDictionary[type];
        }
    }
}
