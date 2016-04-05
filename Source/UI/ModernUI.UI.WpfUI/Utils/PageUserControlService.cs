using NugetWorkflow.Common.Base.Exceptions;
using NugetWorkflow.Common.Base.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NugetWorkflow.UI.WpfUI.Utils
{
    public static class PageUserControlService
    {
        private static Dictionary<Type, object> pageUserControls = new Dictionary<Type, object>();

        public static void AddUserControl(IPageUserControl userControl)
        {
            pageUserControls.Add(userControl.GetType(), userControl);
            userControl.AssignViewModel();
        }

        public static void ReassignViewModels()
        {
            foreach (var pageUserControl in pageUserControls)
            {
                if (pageUserControl.Value is IPageUserControl)
                {
                    ((IPageUserControl)pageUserControl.Value).AssignViewModel();
                }
            }
            foreach (var viewModel in ViewModelService.ViewDictionary)
            {
            }
        }

        public static RET GetUserControl<RET>()
        {
            return (RET)GetUserControl(typeof(RET));
        }

        public static object GetUserControl(Type type)
        {
            var contains = pageUserControls.Keys.Contains(type);
            if (!contains)
            {
                throw new MissingViewException(string.Format("{0} is missing or does not implement {1} interface.", type.FullName, typeof(IPageUserControl).FullName));
            }
            return pageUserControls[type];
        }

        public static IEnumerable<UserControl> GetUserControls(Func<UserControl, bool> condition)
        {
            try
            {
                var contains = pageUserControls.Values.Where(userControl => condition((UserControl)userControl)).Select(userControl=>(UserControl)userControl);
                return contains;
            }
            catch (Exception)
            {
                return new List<UserControl>();
            }
        }
    }
}
